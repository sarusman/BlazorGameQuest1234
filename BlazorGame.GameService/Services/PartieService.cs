// BlazorGame.GameService/Services/PartieService.cs
using BlazorGame.GameService.Persistence;
using Microsoft.EntityFrameworkCore;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Scores;
using SharedModels.Domain.Common.Enums;

namespace BlazorGame.GameService.Services
{
    /// <summary>Gère une partie : démarrer, appliquer un choix, terminer et enregistrer le score (progressif et final).</summary>
    public class PartieService
    {
        private readonly GameDbContext _db;

        /// <summary>Construit le service.</summary>
        /// <param name="db">DbContext.</param>
        public PartieService(GameDbContext db)
        {
            _db = db;
        }

        /// <summary>Démarre une partie (bloque le rejouage même joueur/même donjon) et crée une ligne de score initiale.</summary>
        /// <param name="joueurId">Id joueur.</param>
        /// <param name="donjonId">Id donjon.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Partie créée ou null si impossible.</returns>
        public async Task<Partie?> DemarrerAsync(Guid joueurId, Guid donjonId, CancellationToken ct)
        {
            var deja = await _db.Parties.AnyAsync(p => p.JoueurId == joueurId && p.DonjonId == donjonId, ct);
            if (deja) return null;

            var d = await _db.Donjons.Include(x => x.Salles).FirstOrDefaultAsync(x => x.Id == donjonId, ct);
            if (d == null) return null;
            if (d.Salles == null || d.Salles.Count == 0) return null;

            var p = new Partie
            {
                Id = Guid.NewGuid(),
                JoueurId = joueurId,
                DonjonId = donjonId,
                DemarreeLe = DateTime.UtcNow,
                EstTerminee = false,
                ScoreFinal = 10
            };

            await _db.Parties.AddAsync(p, ct);
            await _db.SaveChangesAsync(ct);

            var sInit = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = joueurId,
                PartieId = p.Id,
                Valeur = 10,
                EnregistreLe = DateTime.UtcNow
            };
            await _db.Scores.AddAsync(sInit, ct);

            await _db.SaveChangesAsync(ct);
            return p;
        }

        /// <summary>Applique un choix, met à jour le score, enregistre l’étape et persiste le score (progressif puis final si fini).</summary>
        /// <param name="partieId">Id partie.</param>
        /// <param name="salleId">Id salle.</param>
        /// <param name="choixId">Id choix.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>(score, mort, fini, nextSalleId)</returns>
        public async Task<(int score, bool mort, bool fini, Guid? nextSalleId)> AppliquerChoixAsync(Guid partieId, Guid salleId, Guid choixId, CancellationToken ct)
        {
            var p = await _db.Parties.Include(x => x.Etapes).FirstOrDefaultAsync(x => x.Id == partieId, ct);
            if (p == null) return (0, false, true, null);
            if (p.EstTerminee) return (p.ScoreFinal, false, true, null);

            var d = await _db.Donjons
                .Include(x => x.Salles)
                .ThenInclude(s => s.ChoixProposes)
                .ThenInclude(c => c.Effets)
                .FirstOrDefaultAsync(x => x.Id == p.DonjonId, ct);
            if (d == null) return (p.ScoreFinal, false, true, null);
            if (d.Salles == null || d.Salles.Count == 0) return (p.ScoreFinal, false, true, null);

            var list = d.Salles.ToList();
            var s = list.FirstOrDefault(x => x.Id == salleId);
            if (s == null) return (p.ScoreFinal, false, p.EstTerminee, null);

            var deja = p.Etapes.Any(e => e.SalleId == salleId);
            if (deja) return (p.ScoreFinal, false, p.EstTerminee, null);

            var c = s.ChoixProposes?.FirstOrDefault(x => x.Id == choixId);
            if (c == null) return (p.ScoreFinal, false, p.EstTerminee, null);

            int delta = 0;
            bool mort = false;

            if (c.Effets != null)
            {
                foreach (var ef in c.Effets)
                {
                    if (ef.Type == TypeEffet.MortInstantanee) mort = true;
                    else if (ef.Type == TypeEffet.GainPoints) delta = delta + ef.Valeur;
                    else if (ef.Type == TypeEffet.PertePoints) delta = delta + ef.Valeur;
                }
            }

            int scoreApres;
            if (mort) scoreApres = -1;
            else scoreApres = p.ScoreFinal + delta;

            bool mortIci;
            if (mort) mortIci = true;
            else mortIci = scoreApres < 0;

            var etape = new EtapeDePartie
            {
                Id = Guid.NewGuid(),
                PartieId = p.Id,
                SalleId = s.Id,
                Ordre = p.Etapes.Count + 1,
                ChoixId = c.Id,
                ScoreApres = scoreApres,
                MortIci = mortIci
            };
            await _db.Etapes.AddAsync(etape, ct);

            p.Etapes.Add(etape);
            p.ScoreFinal = scoreApres;

            bool fini;
            if (mortIci) fini = true;
            else fini = etape.Ordre >= list.Count;

            p.EstTerminee = fini;

            var ligne = await _db.Scores.FirstOrDefaultAsync(x => x.PartieId == p.Id, ct);
            if (ligne == null)
            {
                ligne = new Score
                {
                    Id = Guid.NewGuid(),
                    JoueurId = p.JoueurId,
                    PartieId = p.Id,
                    Valeur = scoreApres < 0 ? 0 : scoreApres,
                    EnregistreLe = DateTime.UtcNow
                };
                await _db.Scores.AddAsync(ligne, ct);
            }
            else
            {
                ligne.Valeur = scoreApres < 0 ? 0 : scoreApres;
                ligne.EnregistreLe = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);

            Guid? nextId = null;
            if (!p.EstTerminee)
            {
                int idx = list.FindIndex(x => x.Id == s.Id);
                if (idx >= 0)
                {
                    int nextIndex = idx + 1;
                    if (nextIndex < list.Count) nextId = list[nextIndex].Id;
                }
            }

            return (p.ScoreFinal, mortIci, p.EstTerminee, nextId);
        }
    }
}
