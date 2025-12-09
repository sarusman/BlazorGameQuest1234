using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorGame.Tests.ClientTests.NewAdventureTests
{
    public class TestAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly string _userName;
        public TestAuthenticationStateProvider(string userName)
        {
            _userName = userName;
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _userName)
            }, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
