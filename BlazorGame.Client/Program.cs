using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorGame.Client;
using BlazorGame.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<GameState>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ITokenService, LocalStorageTokenService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Configurer le DelegatingHandler pour ajouter le token JWT automatiquement
builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5001")
    };
});

await builder.Build().RunAsync();
