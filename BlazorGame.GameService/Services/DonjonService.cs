using BlazorGame.GameService.Persistence;
using Microsoft.EntityFrameworkCore;
using SharedModels.Domain.Common;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.GameService.Services
{
    /// <summary>Gestion des donjons.</summary>
    public class DonjonService
    {
        private readonly Repository<Donjon> _donjons;
        private readonly SalleService _salles;
        private readonly GameDbContext _db;

        /// <summary>Construit le service.</summary>
        /// <param name="donjons">Repository Donjon.</param>
        /// <param name="salles">Générateur de salle.</param>
        /// <param name="db">DbContext.</param>
        public DonjonService(Repository<Donjon> donjons, SalleService salles, GameDbContext db)
        {
            _donjons = donjons;
            _salles = salles;
            _db = db;
        }

        /// <summary>Crée un donjon avec salles uniques (2/3/5 selon difficulté).</summary>
        /// <param name="nom">Nom.</param>
        /// <param name="difficulte">Difficulté.</param>
        /// <param name="nbSalles">Force le nombre si &gt;0.</param>
        /// <param name="seed">Graine.</param>
        /// <param name="ct">Annulation.</param>
        /// <returns>Donjon créé.</returns>
        public async Task<Donjon> CreateAsync(string nom, Difficulte difficulte, int nbSalles, int? seed, CancellationToken ct)
        {
            var rand = seed.HasValue ? new Random(seed.Value) : new Random();
            var target = nbSalles > 0 ? nbSalles : NbParDifficulte(difficulte);

            var d = new Donjon
            {
                Id = Guid.NewGuid(),
                Nom = string.IsNullOrWhiteSpace(nom) ? "Donjon" : nom,
                Difficulte = difficulte,
                NbMaxSalles = target,
                Salles = new List<Salle>()
            };

            var signatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            const int maxAttempts = 20;

            for (int i = 0; i < target; i++)
            {
                int attempts = 0;
                while (true)
                {
                    attempts++;
                    var salle = _salles.GenererSalle(difficulte, null, rand.Next());
                    var sig = $"{salle.Type}|{salle.Titre}";
                    if (signatures.Add(sig))
                    {
                        d.Salles.Add(salle);
                        break;
                    }
                    if (attempts >= maxAttempts)
                    {
                        salle.Titre = $"{salle.Titre} ({Guid.NewGuid().ToString()[..4]})";
                        d.Salles.Add(salle);
                        break;
                    }
                }
            }

            await _donjons.AddAsync(d, ct);
            return d;
        }

        /// <summary>Retourne un donjon avec salles, choix et effets.</summary>
        /// <param name="id">Id.</param>
        /// <param name="ct">Annulation.</param>
        /// <returns>Donjon ou null.</returns>
        public Task<Donjon?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Donjons
               .Include(d => d.Salles)
               .ThenInclude(s => s.ChoixProposes)
               .ThenInclude(c => c.Effets)
               .AsNoTracking()
               .FirstOrDefaultAsync(d => d.Id == id, ct);

        /// <summary>Nombre de salles recommandé par difficulté.</summary>
        /// <param name="d">Difficulté.</param>
        /// <returns>2/3/5.</returns>
        private static int NbParDifficulte(Difficulte d) => d switch
        {
            Difficulte.Facile => 2,
            Difficulte.Normal => 3,
            Difficulte.Difficile => 5,
            _ => 3
        };
    }
}
