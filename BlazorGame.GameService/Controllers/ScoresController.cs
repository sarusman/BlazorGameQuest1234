// BlazorGame.GameService/Controllers/ScoresController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Scores;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>Endpoints scores (création, lecture, leaderboard).</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly GameDbContext _db;

        /// <summary>Construit le contrôleur.</summary>
        /// <param name="db">DbContext.</param>
        public ScoresController(GameDbContext db)
        {
            _db = db;
        }

        /// <summary>Crée un score manuel.</summary>
        /// <param name="request">JoueurId, PartieId, Valeur.</param>
        /// <param name="ct">Annulation.</param>
        /// <returns>Score créé.</returns>
        [HttpPost]
        public async Task<ActionResult<Score>> Post([FromBody] ScoreRequest request, CancellationToken ct)
        {
            var s = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = request.JoueurId,
                PartieId = request.PartieId,
                Valeur = request.Valeur,
                EnregistreLe = DateTime.UtcNow
            };
            await _db.Scores.AddAsync(s, ct);
            await _db.SaveChangesAsync(ct);
            return Ok(s);
        }

        /// <summary>Retourne tous les scores.</summary>
        /// <param name="ct">Annulation.</param>
        /// <returns>Liste des scores.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Score>>> GetAll(CancellationToken ct)
        {
            var list = await _db.Scores.AsNoTracking()
                .OrderByDescending(x => x.EnregistreLe)
                .ToListAsync(ct);
            return Ok(list);
        }

        /// <summary>Top 10 par valeur décroissante.</summary>
        /// <param name="ct">Annulation.</param>
        /// <returns>Top 10.</returns>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<object>>> Leaderboard(CancellationToken ct)
        {
            var list = await _db.Scores.AsNoTracking()
                .OrderByDescending(s => s.Valeur)
                .ThenBy(s => s.EnregistreLe)
                .Take(10)
                .Select(s => new { s.JoueurId, s.PartieId, s.Valeur, s.EnregistreLe })
                .ToListAsync(ct);

            return Ok(list);
        }

        /// <summary>Payload création score.</summary>
        public class ScoreRequest
        {
            /// <summary>Id joueur.</summary>
            public Guid JoueurId { get; set; }
            /// <summary>Id partie.</summary>
            public Guid PartieId { get; set; }
            /// <summary>Valeur.</summary>
            public int Valeur { get; set; }
        }
    }
}
