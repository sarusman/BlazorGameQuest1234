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

        [HttpPost]
        public async Task<ActionResult<Partie>> Demarrer([FromBody] StartPartieRequest request, CancellationToken ct)
        {
            var p = await _service.DemarrerAsync(request.JoueurId, request.DonjonId, ct);
            return p is null ? Conflict("Partie déjà existante pour ce donjon.") : Ok(p);
        }

        [HttpPost("{id:guid}/choisir")]
        public async Task<ActionResult<ChoisirResponse>> Choisir(Guid id, [FromBody] ChoisirRequest request, CancellationToken ct)
        {
            var (score, mort, fini, nextSalleId) = await _service.AppliquerChoixAsync(id, request.SalleId, request.ChoixId, ct);
            return Ok(new ChoisirResponse { Score = score, Mort = mort, Fini = fini, NextSalleId = nextSalleId });
        }

        public class StartPartieRequest
        {
            public Guid JoueurId { get; set; }
            public Guid DonjonId { get; set; }
        }
        public class ChoisirRequest
        {
            public Guid SalleId { get; set; }
            public Guid ChoixId { get; set; }
        }
        public class ChoisirResponse
        {
            public int Score { get; set; }
            public bool Mort { get; set; }
            public bool Fini { get; set; }
            public Guid? NextSalleId { get; set; }
        }
    }
}
