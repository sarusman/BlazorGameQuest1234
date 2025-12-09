using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(ITokenService tokenService, ILogger<CustomAuthStateProvider> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetTokenAsync();

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation($"Token : {token}");
        _logger.LogInformation($"Claims : {string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}"))}");

        return new AuthenticationState(user);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(PadBase64(payload));
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);

        if (keyValuePairs == null)
            return Enumerable.Empty<Claim>();

        var claims = new List<Claim>();

        foreach (var kvp in keyValuePairs)
        {
            // Extraire les rôles Keycloak depuis realm_access.roles
            if (kvp.Key == "realm_access" && kvp.Value.ValueKind == JsonValueKind.Object)
            {
                if (kvp.Value.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrEmpty(roleValue))
                        {
                            // Ajouter les rôles comme ClaimTypes.Role pour que [Authorize(Roles = "...")] fonctionne
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }
            // Ajouter le preferred_username comme ClaimTypes.Name
            else if (kvp.Key == "preferred_username")
            {
                var username = kvp.Value.GetString();
                if (!string.IsNullOrEmpty(username))
                {
                    claims.Add(new Claim(ClaimTypes.Name, username));
                }
            }
            // Ajouter tous les autres claims
            else
            {
                var value = kvp.Value.ValueKind == JsonValueKind.String 
                    ? kvp.Value.GetString() 
                    : kvp.Value.ToString();
                
                if (!string.IsNullOrEmpty(value))
                {
                    claims.Add(new Claim(kvp.Key, value));
                }
            }
        }

        return claims;
    }

    private string PadBase64(string base64)
    {
        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }
    
    public async Task Logout()
    {
        await _tokenService.RemoveTokenAsync();
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}
