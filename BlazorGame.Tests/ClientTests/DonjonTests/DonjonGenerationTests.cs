using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorGame.Client.Pages.Player.Game.Donjons.DonjonGeneratePage;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ClientTests
{
    public class DonjonGenerationTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
        }

        [Fact]
        public void DonjonGeneration_NavigatesToDonjon()
        {
            var donjonId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId };
            var fakeHandler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(fakeHandler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var uri = nav.GetUriWithQueryParameter("level", 1);
            nav.NavigateTo(uri);

            var cut = ctx.Render<DonjonGeneration>();

            Assert.Contains($"/donjon/{donjonId}", nav.Uri);
        }
    }
}



