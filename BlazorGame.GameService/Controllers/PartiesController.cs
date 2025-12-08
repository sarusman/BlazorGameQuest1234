using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartiesController : ControllerBase
    {
        private readonly PartieService _service;

        public PartiesController(PartieService service) { _service = service; }

        /// <summary>
        /// Démarre une nouvelle partie.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Partie>> Demarrer([FromBody] StartPartieRequest request, CancellationToken ct)
        {
            var p = await _service.DemarrerAsync(request.JoueurId, request.DonjonId, ct);
            return p is null ? Conflict("Partie déjà existante pour ce donjon.") : Ok(p);
        }

        /// <summary>
        /// Applique un choix dans une salle pour une partie donnée.
        /// </summary>
        [HttpPost("{id:guid}/choisir")]
        public async Task<ActionResult<ChoisirResponse>> Choisir(Guid id, [FromBody] ChoisirRequest request, CancellationToken ct)
        {
            var (score, mort, fini, nextSalleId) = await _service.AppliquerChoixAsync(id, request.SalleId, request.ChoixId, ct);
            return Ok(new ChoisirResponse { Score = score, Mort = mort, Fini = fini, NextSalleId = nextSalleId });
        }
    }
}
