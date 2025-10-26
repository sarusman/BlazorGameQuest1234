using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Users;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints liés aux parties (démarrer une partie, lire une partie).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PartiesController : ControllerBase
    {
        private readonly Repository<Joueur> _joueurRepo;
        private readonly Repository<Partie> _partieRepo;
        private readonly GameplayService _gameplay;

        /// <summary>
        /// Construit le contrôleur des parties.
        /// </summary>
        /// <param name="joueurRepo">Repository Joueur pour retrouver le joueur.</param>
        /// <param name="partieRepo">Repository Partie pour persister la session de jeu.</param>
        /// <param name="gameplay">Service gameplay pour générer la partie et le donjon.</param>
        public PartiesController(
            Repository<Joueur> joueurRepo,
            Repository<Partie> partieRepo,
            GameplayService gameplay)
        {
            _joueurRepo = joueurRepo;
            _partieRepo = partieRepo;
            _gameplay = gameplay;
        }

        /// <summary>
        /// Crée une nouvelle partie pour un joueur donné.
        /// </summary>
        /// <param name="request">Contient l'Id du joueur qui commence à jouer.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>La partie créée et stockée, avec son donjon généré.</returns>
        [HttpPost("start")]
        public async Task<ActionResult<Partie>> Start([FromBody] StartRequest request, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(request.JoueurId, ct);
            if (joueur == null)
                return NotFound("Joueur introuvable");

            var partie = _gameplay.CreerNouvellePartie(joueur);

            await _partieRepo.AddAsync(partie, ct);

            return Ok(partie);
        }

        /// <summary>
        /// Récupère l'état d'une partie.
        /// </summary>
        /// <param name="id">Id de la partie (GUID).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>La partie si trouvée, sinon 404.</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Partie>> GetById(Guid id, CancellationToken ct)
        {
            var partie = await _partieRepo.GetByIdAsync(id, ct);
            if (partie == null)
                return NotFound();

            return Ok(partie);
        }

        /// <summary>
        /// Données envoyées pour démarrer une partie.
        /// </summary>
        public class StartRequest
        {
            /// <summary>Id du joueur qui lance une nouvelle partie.</summary>
            public Guid JoueurId { get; set; }
        }
    }
}
