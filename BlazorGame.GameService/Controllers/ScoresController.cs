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
        /// <summary>
        /// Crée un score manuel.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Score>> Post([FromBody] ScoreRequest request, CancellationToken ct)
        {
            var s = await _service.CreateAsync(request.JoueurId, request.PartieId, request.Valeur, ct);
            return Ok(s);
        }

        /// <summary>Retourne tous les scores.</summary>
        /// <summary>
        /// Retourne tous les scores.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Score>>> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }

        /// <summary>Top 10 global.</summary>
        /// <summary>
        /// Top 10 global (leaderboard).
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<object>>> Leaderboard(CancellationToken ct)
        {
            var board = await _service.GetLeaderboardAsync(ct);
            return Ok(board);
        }

        /// <summary>Score final d’un donjon.</summary>
        /// <param name="donjonId">Id du donjon.</param>
        /// <summary>
        /// Score final d’un donjon.
        /// </summary>
        [HttpGet("score/{donjonId:guid}")]
        public async Task<ActionResult<Score?>> ByDonjon(Guid donjonId, CancellationToken ct)
        {
            var score = await _service.GetByDonjonAsync(donjonId, ct);
            return score is null ? NotFound() : Ok(score);
        }

        /// <summary>Historique détaillé : scores, pseudo, inventaire final.</summary>
        /// <summary>
        /// Historique détaillé : scores, pseudo, inventaire final.
        /// </summary>
        [HttpGet("history-full")]
        public async Task<ActionResult<List<object>>> GetHistoryFull(CancellationToken ct)
        {
            var scores = await _service.GetAllAsync(ct);
            
            var joueurs = scores.Select(s => s.JoueurId).Distinct().ToList();
            var db = HttpContext.RequestServices.GetService(typeof(BlazorGame.GameService.Persistence.GameDbContext)) as BlazorGame.GameService.Persistence.GameDbContext;
            var joueursDict = db!.Joueurs.ToDictionary(j => j.Id, j => j);

            var inventaires = db!.Set<SharedModels.Domain.Items.InventaireItem>().ToList();
            var objets = db!.Set<SharedModels.Domain.Items.Objet>().ToList();


            var result = scores.Select(s => new {
                Joueur = joueursDict.TryGetValue(s.JoueurId, out var j) ? j.Pseudo : (j?.Pseudo ?? "?"),
                Score = s.Valeur,
                Date = s.EnregistreLe,
                Inventaire = inventaires.Where(ii => ii.JoueurId == s.JoueurId)
                    .Select(ii => new {
                        Objet = objets.FirstOrDefault(o => o.Id == ii.ObjetId)?.Nom ?? "?",
                        ii.Quantite
                    }).ToList()
            }).ToList();
            return Ok(result);
        }
    }
}
