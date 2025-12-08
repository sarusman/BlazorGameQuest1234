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
await builder.Build().RunAsync();
