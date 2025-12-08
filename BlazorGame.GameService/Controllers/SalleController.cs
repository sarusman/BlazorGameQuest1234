using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Common.Enums;
using BlazorGame.GameService.Filters;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints de génération de salle (aléatoire).
    /// </summary>
    [ApiController]
    [RequireLogin]
    [Route("api/[controller]")]
    public class SalleController : ControllerBase
    {
        private readonly SalleService _salleService;

        /// <summary>
        /// Construit le contrôleur des salles.
        /// </summary>
        /// <param name="salleService">Service de génération de salles.</param>
        public SalleController(SalleService salleService)
        {
            _salleService = salleService;
        }

        /// <summary>
        /// Génère une salle aléatoire.
        /// </summary>
        /// <param name="difficulte">Difficulté (Facile/Normal/Difficile/Mortel).</param>
        /// <param name="type">Type de salle (Combat, Coffre, ...). Laisse null pour aléatoire.</param>
        /// <param name="seed">Graine RNG optionnelle (pour résultat reproductible).</param>
        /// <returns>La salle générée.</returns>
        /// <summary>
        /// Génère une salle aléatoire.
        /// </summary>
        [HttpGet("random")]
        public ActionResult<Salle> Random(
            [FromQuery] Difficulte difficulte = Difficulte.Normal,
            [FromQuery] TypeSalle? type = null,
            [FromQuery] int? seed = null)
        {
            var salle = _salleService.GenererSalle(difficulte, type, seed);
            return Ok(salle);
        }
    }
}
