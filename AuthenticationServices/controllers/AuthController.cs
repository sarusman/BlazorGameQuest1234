using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;


namespace AuthenticationServices.Controllers
{
    /// <summary>
    /// Contrôleur d'authentification gérant les requêtes liées à la connexion et la vérification du service.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

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

        /// <summary>
        /// Authentifie un utilisateur via Keycloak.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var keycloakUrl = _configuration["Keycloak:TokenUrl"] ?? "http://keycloak:8080/realms/blazorgame/protocol/openid-connect/token";
            var clientId = _configuration["Keycloak:ClientId"] ?? "blazorgame-client";
            var clientSecret = _configuration["Keycloak:ClientSecret"] ?? "";

            var httpClient = _httpClientFactory.CreateClient();
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "username", request.Username },
                { "password", request.Password }
            };

            var response = await httpClient.PostAsync(keycloakUrl, new FormUrlEncodedContent(formData));

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return Ok(tokenResponse);
            }

            return Unauthorized("Échec de la connexion. Vérifiez vos identifiants.");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
