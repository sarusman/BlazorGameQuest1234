using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints liés aux joueurs (inscription, login, info).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JoueursController : ControllerBase
    {
        private readonly Repository<Joueur> _joueurRepo;

        /// <summary>
        /// Construit le contrôleur joueur.
        /// </summary>
        /// <param name="joueurRepo">Repository pour Joueur.</param>
        public JoueursController(Repository<Joueur> joueurRepo)
        {
            _joueurRepo = joueurRepo;
        }

        /// <summary>
        /// Inscrit un nouveau joueur.
        /// </summary>
        /// <param name="request">Données d'inscription (pseudo, email).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le joueur créé.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<Joueur>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = request.Pseudo,
            };

            await _joueurRepo.AddAsync(joueur, ct);
            return Ok(joueur);
        }

        /// <summary>
        /// Simule la connexion d'un joueur par pseudo.
        /// </summary>
        /// <param name="request">Pseudo du joueur qui veut se connecter.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le joueur si trouvé, sinon 404.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Joueur>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var all = await _joueurRepo.ListAsync(ct);
            var joueur = all.FirstOrDefault(j => j.Pseudo == request.Pseudo);

            if (joueur == null)
                return NotFound("Joueur introuvable");

            return Ok(joueur);
        }

        /// <summary>
        /// Retourne les infos d'un joueur.
        /// </summary>
        /// <param name="id">Id du joueur.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le joueur si trouvé, sinon 404.</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Joueur>> GetById(Guid id, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(id, ct);
            if (joueur == null)
                return NotFound();

            return Ok(joueur);
        }
    }
}


