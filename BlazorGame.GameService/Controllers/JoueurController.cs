using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;

namespace BlazorGame.GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JoueursController : ControllerBase
    {
        private readonly Repository<Joueur> _joueurRepo;

        public JoueursController(Repository<Joueur> joueurRepo)
        {
            _joueurRepo = joueurRepo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Joueur>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            Console.WriteLine($"[REGISTER] Pseudo reçu: '{request.Pseudo}'");
            var existant = (await _joueurRepo.ListAsync(ct)).FirstOrDefault(j => j.Pseudo == request.Pseudo);
            if (existant != null)
            {
                Console.WriteLine($"[REGISTER] Pseudo déjà existant: '{request.Pseudo}'");
                return Conflict("Ce pseudo existe déjà. Veuillez en choisir un autre.");
            }

            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = request.Pseudo,
                Actif = true
            };

            await _joueurRepo.AddAsync(joueur, ct);

            Console.WriteLine($"Test Id={joueur.Id}, Pseudo='{joueur.Pseudo}'");

            Response.Cookies.Append("pseudo", joueur.Pseudo, new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            Response.Cookies.Append("joueurId", joueur.Id.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return Ok(joueur);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Joueur>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var all = await _joueurRepo.ListAsync(ct);
            var joueur = all.FirstOrDefault(j => j.Pseudo == request.Pseudo);

            if (joueur == null) return NotFound("Joueur introuvable");
            if (!joueur.Actif) return NotFound("Joueur désactivé");

            Response.Cookies.Append("pseudo", joueur.Pseudo, new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            Response.Cookies.Append("joueurId", joueur.Id.ToString(), new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return Ok(joueur);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("pseudo", new CookieOptions { Path = "/", SameSite = SameSiteMode.None, Secure = true });
            Response.Cookies.Delete("joueurId", new CookieOptions { Path = "/", SameSite = SameSiteMode.None, Secure = true });
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Joueur>> GetById(Guid id, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(id, ct);
            if (joueur == null) return NotFound();
            if (!joueur.Actif) return NotFound("Joueur désactivé par un admin");
            return Ok(joueur);
        }

        [HttpPut("{id:guid}/actif")]
        public async Task<ActionResult<Joueur>> UpdateActif(Guid id, [FromBody] bool actif, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(id, ct);
            if (joueur == null) return NotFound();

            joueur.Actif = actif;
            await _joueurRepo.UpdateAsync(joueur, ct);
            return Ok(joueur);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Joueur>>> ListAsync(CancellationToken ct)
        {
            return Ok(await _joueurRepo.ListAsync(ct));
        }
    }

}
