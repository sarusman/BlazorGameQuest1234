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
        /// Retourne l’unique score final associé à un donjon.
        /// Comme une partie terminée écrit un seul score final, on renvoie
        /// le score le plus récent trouvé pour ce donjon (ou null s’il n’y en a pas).
        /// </summary>
        /// <param name="donjonId">Id du donjon.</param>
        /// <returns>Le score final du donjon, ou null.</returns>
        public Task<Score?> GetByDonjonAsync(Guid donjonId, CancellationToken ct) =>
            _db.Scores.AsNoTracking()
            .Join(_db.Parties.AsNoTracking(),
                    s => s.PartieId,
                    p => p.Id,
                    (s, p) => new { s, p })
            .Where(x => x.p.DonjonId == donjonId)
            .OrderByDescending(x => x.s.EnregistreLe)
            .Select(x => x.s)
            .FirstOrDefaultAsync(ct);
    }
}
