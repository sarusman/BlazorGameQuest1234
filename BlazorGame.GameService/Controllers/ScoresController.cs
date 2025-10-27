using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Scores;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints liés aux scores (sauvegarde du score final d'une partie + leaderboard).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly Repository<Score> _scoreRepo;

        /// <summary>
        /// Construit le contrôleur score.
        /// </summary>
        /// <param name="scoreRepo">Repository Score pour lecture/écriture.</param>
        public ScoresController(Repository<Score> scoreRepo)
        {
            _scoreRepo = scoreRepo;
        }

        /// <summary>
        /// Enregistre le score final d'une partie jouée.
        /// </summary>
        /// <param name="request">JoueurId, PartieId, valeur du score.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le score créé.</returns>
        [HttpPost]
        public async Task<ActionResult<Score>> Post([FromBody] ScoreRequest request, CancellationToken ct)
        {
            var score = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = request.JoueurId,
                PartieId = request.PartieId,
                Valeur = request.Valeur,
                EnregistreLe = DateTime.UtcNow
            };

            await _scoreRepo.AddAsync(score, ct);
            return Ok(score);
        }

        /// <summary>
        /// Retourne tous les scores enregistrés.
        /// </summary>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Liste de tous les scores.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Score>>> GetAll(CancellationToken ct)
        {
            var list = await _scoreRepo.ListAsync(ct);
            return Ok(list);
        }

        /// <summary>
        /// Retourne le top 10 des meilleurs scores (leaderboard global).
        /// </summary>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Liste des meilleurs scores triés par valeur décroissante.</returns>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<object>>> GetLeaderboard(CancellationToken ct)
        {
            var list = await _scoreRepo.ListAsync(ct);

            var board = list
                .OrderByDescending(s => s.Valeur)
                .Take(10)
                .Select(s => new
                {
                    s.JoueurId,
                    s.PartieId,
                    s.Valeur,
                    s.EnregistreLe
                })
                .ToList();

            return Ok(board);
        }

        /// <summary>
        /// Modèle pour créer un nouveau score.
        /// </summary>
        public class ScoreRequest
        {
            /// <summary>Id du joueur lié à ce score.</summary>
            public Guid JoueurId { get; set; }

            /// <summary>Id de la partie jouée.</summary>
            public Guid PartieId { get; set; }

            /// <summary>Valeur numérique du score final.</summary>
            public int Valeur { get; set; }
        }
    }
}


