using Microsoft.AspNetCore.Mvc;


namespace AuthenticationServices.Controllers
{
    /// <summary>
    /// Contrôleur d'authentification gérant les requêtes liées à la connexion et la vérification du service.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Vérifie si le service d'authentification est en ligne.
        /// </summary>
        /// <returns>
        /// Retourne un code 200 (OK) avec un message indiquant que le service est disponible.
        /// </returns>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Vous pouvez vous connecter !");
        }
    }
}
