using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Xunit;
using BlazorGame.Client.Pages.Player.Game.Donjons.DonjonView;
using BlazorGame.Client.Services;
using SharedModels.Domain.Gameplay;
using System.Collections.Generic;

namespace BlazorGame.Tests.ClientTests
{
    public class DonjonViewTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
        }

        [Fact]
        public void DonjonView_StartsAndNavigates()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var donjon = new Donjon
            {
                Id = donjonId,
                Nom = "D",
                Salles = new List<Salle> { new Salle { Id = salleId } }
            };

            var fakeHandler = new FakeHandler(req =>
            {
                if (req.Method == HttpMethod.Get)
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) };
                if (req.Method == HttpMethod.Post)
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new Partie { Id = Guid.NewGuid() }) };
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(fakeHandler) { BaseAddress = new Uri("http://localhost") });
            ctx.Services.AddSingleton<GameState>();

            var cut = ctx.Render<DonjonView>(parameters => parameters.Add(p => p.id, donjonId));

            var button = cut.Find(".start-btn");
            button.Click();

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            Assert.Contains($"/salle/{donjonId}", nav.Uri);
        }
    }
}
