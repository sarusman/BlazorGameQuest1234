using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Sealed;

namespace BlazorGame.GameService.Controllers
{
    /// <summary>
    /// Endpoints de création et récupération des donjons.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DonjonsController : ControllerBase
    {
        private readonly DonjonService _service;

        /// <summary>
        /// Construit le contrôleur des donjons.
        /// </summary>
        /// <param name="service">Service de donjon.</param>
        public DonjonsController(DonjonService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crée un donjon avec un nombre donné de salles.
        /// </summary>
        /// <param name="request">Nom, difficulté, nombre de salles, seed.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le donjon créé.</returns>
        [HttpPost]
        public async Task<ActionResult<Donjon>> Create([FromBody] CreateDonjonRequest request, CancellationToken ct)
        {
            var d = await _service.CreateAsync(
                request.Nom ?? "Donjon",
                request.Difficulte,
                request.NbSalles,
                request.Seed,
                ct);

            return Ok(d);
        }

        /// <summary>
        /// Récupère un donjon par son Id.
        /// </summary>
        /// <param name="id">Id du donjon (GUID).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Le donjon si trouvé, sinon 404.</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Donjon>> GetById(Guid id, CancellationToken ct)
        {
            var d = await _service.GetByIdAsync(id, ct);
            return d is null ? NotFound() : Ok(d);
        }

    }
}
