// BlazorGame.GameService/Controllers/ScoresController.cs
using Microsoft.AspNetCore.Mvc;
using SharedModels.Domain.Scores;
using BlazorGame.GameService.Services;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>Endpoints scores (création, lecture, leaderboard).</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly ScoresService _service;

        /// <summary>Construit le contrôleur.</summary>
        /// <param name="service">Service des scores.</param>
        public ScoresController(ScoresService service) => _service = service;

        /// <summary>Crée un score manuel.</summary>
        [HttpPost]
        public async Task<ActionResult<Score>> Post([FromBody] ScoreRequest request, CancellationToken ct)
        {
            var s = await _service.CreateAsync(request.JoueurId, request.PartieId, request.Valeur, ct);
            return Ok(s);
        }

        /// <summary>Retourne tous les scores.</summary>
        [HttpGet]
        public async Task<ActionResult<List<Score>>> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }

        /// <summary>Top 10 global.</summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<object>>> Leaderboard(CancellationToken ct)
        {
            var board = await _service.GetLeaderboardAsync(ct);
            return Ok(board);
        }

        /// <summary>Score final d’un donjon.</summary>
        /// <param name="donjonId">Id du donjon.</param>
        [HttpGet("score/{donjonId:guid}")]
        public async Task<ActionResult<Score?>> ByDonjon(Guid donjonId, CancellationToken ct)
        {
            var score = await _service.GetByDonjonAsync(donjonId, ct);
            return score is null ? NotFound() : Ok(score);
        }

        /// <summary>Payload création score.</summary>
        public class ScoreRequest
        {
            public Guid JoueurId { get; set; }
            public Guid PartieId { get; set; }
            public int Valeur { get; set; }
        }
    }
}
