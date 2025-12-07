using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorGame.Client;
using BlazorGame.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<GameState>();

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:8080")
    });

var authBuilder = builder.Services.AddAuthentication("Keycloak");
authBuilder.AddOpenIdConnect(
    authenticationScheme: "Keycloak",
    options =>
    {
        options.ClientId = "gamequest-frontend";
        options.ClientSecret = "9K9DMyoTc43pIRVLV6kvxW7uW5EpSStO";
        options.Authority = "http://localhost:8180/realms/gamequest";
        options.Scope.Add("gmaequest-backend.all");
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.SignOutScheme = "Keycloak";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
        options.MapInboundClaims = false;
    }
);

authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddAuthorizationBuilder();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

await builder.Build().RunAsync();
