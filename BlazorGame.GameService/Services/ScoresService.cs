// BlazorGame.GameService/Services/ScoresService.cs
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Scores;

namespace BlazorGame.GameService.Services
{
    /// <summary>Service d’accès/gestion des scores.</summary>
    public class ScoresService
    {
        private readonly GameDbContext _db;

        /// <summary>Construit le service.</summary>
        /// <param name="db">DbContext.</param>
        public ScoresService(GameDbContext db) => _db = db;

        /// <summary>Crée un score.</summary>
        public async Task<Score> CreateAsync(Guid joueurId, Guid partieId, int valeur, CancellationToken ct)
        {
            var s = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = joueurId,
                PartieId = partieId,
                Valeur = valeur,
                EnregistreLe = DateTime.UtcNow
            };
            await _db.Scores.AddAsync(s, ct);
            await _db.SaveChangesAsync(ct);
            return s;
        }

        /// <summary>Retourne tous les scores (du plus récent au plus ancien).</summary>
        public Task<List<Score>> GetAllAsync(CancellationToken ct) =>
            _db.Scores.AsNoTracking()
              .OrderByDescending(x => x.EnregistreLe)
              .ToListAsync(ct);

        /// <summary>Retourne le top 10 global des scores.</summary>
        public Task<List<object>> GetLeaderboardAsync(CancellationToken ct) =>
            _db.Scores.AsNoTracking()
              .OrderByDescending(s => s.Valeur)
              .ThenBy(s => s.EnregistreLe)
              .Take(10)
              .Select(s => new { s.JoueurId, s.PartieId, s.Valeur, s.EnregistreLe })
              .Cast<object>()
              .ToListAsync(ct);

        /// <summary>
        /// Retourne les scores liés à un donjon (via la Partie → DonjonId),
        /// triés par valeur décroissante puis date.
        /// </summary>
        /// <param name="donjonId">Id du donjon.</param>
        /// <param name="ct">Annulation.</param>
        /// <returns>Scores du donjon.</returns>
        public Task<List<object>> GetByDonjonAsync(Guid donjonId, CancellationToken ct) =>
            _db.Scores.AsNoTracking()
              .Join(_db.Parties.AsNoTracking(),
                    s => s.PartieId,
                    p => p.Id,
                    (s, p) => new { s, p })
              .Where(x => x.p.DonjonId == donjonId)
              .OrderByDescending(x => x.s.Valeur)
              .ThenBy(x => x.s.EnregistreLe)
              .Select(x => new { x.s.JoueurId, x.s.PartieId, x.s.Valeur, x.s.EnregistreLe })
              .Cast<object>()
              .ToListAsync(ct);
    }
}
