using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartiesController : ControllerBase
    {
        private readonly PartieService _service;
        private readonly Persistence.GameDbContext _db;

        public PartiesController(PartieService service, Persistence.GameDbContext db) 
        { 
            _service = service;
            _db = db;
        }

        /// <summary>
        /// Démarre une nouvelle partie.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Partie>> Demarrer([FromBody] StartPartieRequest request, CancellationToken ct)
        {
            // Récupérer le token JWT directement depuis les headers
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            string? username = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
                        ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                        ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                        ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value?.Split('@')[0];

                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }

            // Fallback si aucun username trouvé
            if (string.IsNullOrWhiteSpace(username))
            {
                username = "anonymous";
            }


            // Trouver ou créer le joueur avec ce pseudo
            var joueur = await _db.Joueurs.FirstOrDefaultAsync(j => j.Pseudo == username, ct);
            if (joueur == null)
            {
                joueur = new SharedModels.Domain.Users.Joueur
                {
                    Id = Guid.NewGuid(),
                    Pseudo = username,
                    Actif = true
                };
                await _db.Joueurs.AddAsync(joueur, ct);
                await _db.SaveChangesAsync(ct);
            }

            var p = await _service.DemarrerAsync(joueur.Id, request.DonjonId, ct);
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
