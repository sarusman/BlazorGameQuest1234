using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BlazorGame.Client.Services;

namespace BlazorGame.Client
{
    /// <summary>
    /// DelegatingHandler qui ajoute automatiquement le token JWT dans l'en-tête Authorization de chaque requête HTTP.
    /// </summary>
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public AuthorizationMessageHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
