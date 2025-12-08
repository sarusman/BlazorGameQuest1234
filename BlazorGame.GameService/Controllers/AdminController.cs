using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly Repository<Joueur> _joueurRepo;
        private readonly Repository<Partie> _partieRepo;
        private readonly ScoresService _scoresService;

        public AdminController(Repository<Joueur> joueurRepo, Repository<Partie> partieRepo, ScoresService scoresService)
        {
            _joueurRepo = joueurRepo;
            _partieRepo = partieRepo;
            _scoresService = scoresService;
        }

        /// <summary>
        /// Récupère la liste des joueurs.
        /// </summary>
        [HttpGet("joueurs")]
        public async Task<ActionResult<IEnumerable<Joueur>>> GetJoueurs(CancellationToken ct)
        {
            return Ok(await _joueurRepo.ListAsync(ct));
        }

        /// <summary>
        /// Définit l'état actif d'un joueur.
        /// </summary>
        [HttpPut("joueurs/{id:guid}/actif")]
        public async Task<ActionResult<Joueur>> SetJoueurActif(Guid id, [FromBody] bool actif, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(id, ct);
            if (joueur == null) return NotFound();
            joueur.Actif = actif;
            await _joueurRepo.UpdateAsync(joueur, ct);
            return Ok(joueur);
        }

        /// <summary>
        /// Récupère la liste des scores.
        /// </summary>
        [HttpGet("scores")]
        public async Task<ActionResult<IEnumerable<object>>> GetScores(CancellationToken ct)
        {
            var scores = await _scoresService.GetAllAsync(ct);
            return Ok(scores);
        }

        /// <summary>
        /// Récupère le classement général (leaderboard).
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<object>>> GetLeaderboard(CancellationToken ct)
        {
            var leaderboard = await _scoresService.GetLeaderboardAsync(ct);
            return Ok(leaderboard);
        }

        /// <summary>
        /// Récupère la liste des parties.
        /// </summary>
        [HttpGet("parties")]
        public async Task<ActionResult<IEnumerable<Partie>>> GetParties(CancellationToken ct)
        {
            return Ok(await _partieRepo.ListAsync(ct));
        }

        /// <summary>
        /// Exporte la liste des joueurs au format CSV.
        /// </summary>
        [HttpGet("export-joueurs")]
        public async Task<IActionResult> ExportJoueurs(CancellationToken ct)
        {
            var joueurs = await _joueurRepo.ListAsync(ct);
            var csv = string.Join("\n", joueurs.Select(j => $"{j.Id},{j.Pseudo},{j.Actif}"));
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "joueurs.csv");
        }

        /// <summary>
        /// Authentifie un administrateur.
        /// </summary>
        [HttpPost("login")]
        
        public async Task<ActionResult<Joueur>> Login([FromBody] SharedModels.Domain.Users.LoginRequest request, CancellationToken ct)
        {
            var all = await _joueurRepo.ListAsync(ct);
            var admin = all.FirstOrDefault(j => j.Pseudo == request.Pseudo && j.Admin);
            if (admin == null || request.Pseudo != "admin1234") return NotFound("Admin introuvable");
            if (!admin.Actif) return NotFound("Admin désactivé");

            Response.Cookies.Append("pseudo", admin.Pseudo, new Microsoft.AspNetCore.Http.CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            Response.Cookies.Append("joueurId", admin.Id.ToString(), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return Ok(admin);
        }
    }
}
