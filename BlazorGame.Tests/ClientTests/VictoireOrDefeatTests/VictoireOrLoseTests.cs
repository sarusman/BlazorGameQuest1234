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
using BlazorGame.Client.Pages.Player.VictoryDefeat;

namespace BlazorGame.Tests.ClientTests.VictoireOrDefeatTests
{
    public class VictoireOrLoseTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
        }

        [Fact]
        public void ShowsLoadingThenVictoryCard()
        {
            var donjonId = Guid.NewGuid();
            var scoreDto = new { JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 42, EnregistreLe = DateTime.Now };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(scoreDto) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            nav.NavigateTo($"/IsVictory/{donjonId}?victory=true");

            var cut = ctx.Render<VictoireOrLose>(parameters => parameters.Add(p => p.donjonId, donjonId));

            cut.WaitForState(() => cut.Markup.Contains("Score final"));
            Assert.Contains("VICTOIRE", cut.Markup);
            Assert.Contains("42 pts", cut.Markup);
        }

        [Fact]
        public void ShowsDefeatCardWhenVictoryFalse()
        {
            var donjonId = Guid.NewGuid();
            var scoreDto = new { JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 13, EnregistreLe = DateTime.Now };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(scoreDto) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            nav.NavigateTo($"/IsVictory/{donjonId}?victory=false");

            var cut = ctx.Render<VictoireOrLose>(parameters => parameters.Add(p => p.donjonId, donjonId));

            cut.WaitForState(() => cut.Markup.Contains("Score final"));
            Assert.Contains("DEFAITE", cut.Markup);
            Assert.Contains("13 pts", cut.Markup);
            Assert.Contains("Vous êtes tombé au combat", cut.Markup);
        }

        [Fact]
        public void ShowsErrorOnApiFailure()
        {
            var donjonId = Guid.NewGuid();
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.InternalServerError));

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            nav.NavigateTo($"/IsVictory/{donjonId}?victory=true");

            var cut = ctx.Render<VictoireOrLose>(parameters => parameters.Add(p => p.donjonId, donjonId));
            cut.WaitForState(() => cut.Markup.Contains("Erreur:"));
            Assert.Contains("Erreur:", cut.Markup);
        }

        [Fact]
        public void RetryGameAndGoHome_NavigatesCorrectly()
        {
            var donjonId = Guid.NewGuid();
            var scoreDto = new { JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 99, EnregistreLe = DateTime.Now };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(scoreDto) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            nav.NavigateTo($"/IsVictory/{donjonId}?victory=true");

            var cut = ctx.Render<VictoireOrLose>(parameters => parameters.Add(p => p.donjonId, donjonId));
            cut.WaitForState(() => cut.Markup.Contains("Score final"));

            var retryBtn = cut.Find(".btn-retry");
            retryBtn.Click();
            Assert.Contains("/new-adventure", nav.Uri);

            nav.NavigateTo($"/IsVictory/{donjonId}?victory=true");
            cut = ctx.Render<VictoireOrLose>(parameters => parameters.Add(p => p.donjonId, donjonId));
            cut.WaitForState(() => cut.Markup.Contains("Score final"));

            var homeBtn = cut.Find(".btn-home");
            homeBtn.Click();
            Assert.Contains("/", nav.Uri);
        }
    }
}



