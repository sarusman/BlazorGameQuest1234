using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints joueurs : création de joueur, login simulé, récupération d'un joueur.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JoueursController : ControllerBase
    {
        private readonly Repository<Joueur> _joueurRepo;

        /// <summary>
        /// Construit le contrôleur joueur.
        /// </summary>
        /// <param name="joueurRepo">Repository pour l'entité Joueur.</param>
        public JoueursController(Repository<Joueur> joueurRepo)
        {
            _joueurRepo = joueurRepo;
        }

        /// <summary>
        /// Crée un nouveau joueur (inscription / enregistrement initial).
        /// </summary>
        /// <param name="request">Pseudo, compte Keycloak et état Actif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le joueur créé en base.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<Joueur>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = request.Pseudo,
                KeycloakUserName = request.KeycloakUserName,
                Actif = request.Actif
            };

            await _joueurRepo.AddAsync(joueur, ct);
            return Ok(joueur);
        }

        /// <summary>
        /// Simule la connexion d'un joueur via son identifiant Keycloak.
        /// </summary>
        /// <param name="request">Identifiant Keycloak envoyé par le client.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le joueur correspondant si trouvé, sinon 404.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Joueur>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var all = await _joueurRepo.ListAsync(ct);

            var joueur = all.FirstOrDefault(j =>
                j.KeycloakUserName == request.KeycloakUserName &&
                j.Actif
            );

            if (joueur == null)
                return NotFound("Joueur introuvable ou inactif");

            return Ok(joueur);
        }

        /// <summary>
        /// Récupère les infos d'un joueur via son Id.
        /// </summary>
        /// <param name="id">Id du joueur (GUID).</param>
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

        /// <summary>
        /// Modèle d'inscription d'un joueur.
        /// </summary>
        public class RegisterRequest
        {
            /// <summary>Pseudo public affiché en jeu.</summary>
            public string Pseudo { get; set; } = string.Empty;

            /// <summary>Nom d'utilisateur Keycloak lié à ce joueur.</summary>
            public string KeycloakUserName { get; set; } = string.Empty;

            /// <summary>Statut actif / banni.</summary>
            public bool Actif { get; set; } = true;
        }

        /// <summary>
        /// Modèle de login joueur.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>Identifiant Keycloak du joueur courant.</summary>
            public string KeycloakUserName { get; set; } = string.Empty;
        }
    }
}
