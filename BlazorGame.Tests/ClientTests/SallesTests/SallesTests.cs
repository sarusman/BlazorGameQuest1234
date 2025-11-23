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
using BlazorGame.Client.Pages.Player.Game.Salles;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Common.Enums;
using BlazorGame.Client.Services;

namespace BlazorGame.Tests.ClientTests.SallesTests
{
    public class SallesTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
        }

        [Fact]
        public void ReinitialiserSiPremiereSalle_ResetsGameState()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = salleId } } };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            var gs = new GameState(); gs.Apply(10, "start"); gs.ResetForDonjon(donjonId, Guid.NewGuid());
            ctx.Services.AddSingleton(gs);
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));

            Assert.Equal(0, gs.Score);
            Assert.Empty(gs.Journal);
        }

        [Fact]
        public void DefinirUrlVideo_SetsCorrectVideoByType()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var salle = new Salle { Id = salleId, Type = TypeSalle.Coffre };
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { salle } };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
            ctx.Services.AddSingleton(new GameState());

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));

            var markup = cut.Markup;
            Assert.Contains("/videos/coffre.mp4", markup);
            Assert.Contains(salleId.ToString(), markup);
        }

        [Fact]
        public void SelectionnerSalle_NotFound_ShowsError()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var otherId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = otherId } } };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
            ctx.Services.AddSingleton(new GameState());

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));

            Assert.Contains("Erreur de chargement", cut.Markup);
        }

        [Fact]
        public void Choisir_Success_UpdatesScoreAndShowsResult()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var choixId = Guid.NewGuid();
            var choixLib = "Ouvrir";
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = salleId, Titre = "T", ChoixProposes = new System.Collections.Generic.List<Choix> { new Choix { Id = choixId, Libelle = choixLib } } } } };

            var handler = new FakeHandler(req =>
            {
                if (req.Method == HttpMethod.Get) return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) };
                if (req.Method == HttpMethod.Post) return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new { Score = 7, Mort = false, Fini = false, NextSalleId = (Guid?)null }) };
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var ctx = new Bunit.BunitContext();
            var gs = new GameState(); gs.ResetForDonjon(donjonId, Guid.NewGuid()); ctx.Services.AddSingleton(gs);
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));

            var btn = cut.Find(".choice-btn");
            btn.Click();

            Assert.Equal(7, gs.Score);
            Assert.Contains(choixLib, cut.Markup);
            Assert.Contains("result-panel", cut.Markup);
        }

        [Fact]
        public void Choisir_Failure_ShowsError()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var choixId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = salleId, ChoixProposes = new System.Collections.Generic.List<Choix> { new Choix { Id = choixId, Libelle = "X" } } } } };

            var handler = new FakeHandler(req =>
            {
                if (req.Method == HttpMethod.Get) return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) };
                if (req.Method == HttpMethod.Post) return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var ctx = new Bunit.BunitContext();
            var gs = new GameState(); gs.ResetForDonjon(donjonId, Guid.NewGuid()); ctx.Services.AddSingleton(gs);
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));
            var btn = cut.Find(".choice-btn");
            btn.Click();

            Assert.Contains("Erreur:", cut.Markup);
            Assert.Contains("InternalServerError", cut.Markup);
        }

        [Fact]
        public void AllerSalleSuivante_NavigatesToNextSalle()
        {
            var donjonId = Guid.NewGuid();
            var s1 = Guid.NewGuid();
            var s2 = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = s1, ChoixProposes = new System.Collections.Generic.List<Choix> { new Choix { Id = Guid.NewGuid(), Libelle = "Next" } } }, new Salle { Id = s2 } } };

            var handler = new FakeHandler(req =>
            {
                if (req.Method == HttpMethod.Get) return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) };
                if (req.Method == HttpMethod.Post) return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new { Score = 3, Mort = false, Fini = false, NextSalleId = (Guid?)s2 }) };
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            using var ctx = new Bunit.BunitContext();
            var gs = new GameState(); gs.ResetForDonjon(donjonId, Guid.NewGuid()); ctx.Services.AddSingleton(gs);
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, s1));
            var btn = cut.Find(".choice-btn");
            btn.Click();

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var cont = cut.Find(".btn-continue");
            cont.Click();

            Assert.Contains($"/salle/{donjonId}", nav.Uri);
            Assert.Contains(s2.ToString(), nav.Uri);
        }

        [Fact]
        public void AllerVictoire_NavigatesToVictory()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = salleId } } };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new GameState());
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));
            cut.Instance.GetType().GetMethod("AllerVictoire", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(cut.Instance, null);
            Assert.Contains($"/IsVictory/{donjonId}?victory=true", nav.Uri);
        }

        [Fact]
        public void AllerDefaite_NavigatesToDefeat()
        {
            var donjonId = Guid.NewGuid();
            var salleId = Guid.NewGuid();
            var donjon = new Donjon { Id = donjonId, Salles = new System.Collections.Generic.List<Salle> { new Salle { Id = salleId } } };
            var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(donjon) });

            using var ctx = new Bunit.BunitContext();
            ctx.Services.AddSingleton(new GameState());
            ctx.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });

            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var cut = ctx.Render<Salles>(parameters => parameters.Add(p => p.donjonId, donjonId).Add(p => p.idSalle, salleId));
            cut.Instance.GetType().GetMethod("AllerDefaite", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(cut.Instance, null);
            Assert.Contains($"/IsVictory/{donjonId}?victory=false", nav.Uri);
        }
    }
}



