using System;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorGame.Client.Pages.Player.Aventure;

namespace BlazorGame.Tests.ClientTests.NewAdventureTests
{
    public class NewAdventureTests
    {
        [Fact]
        public void RendersLevelButtons()
        {
            using var ctx = new Bunit.BunitContext();
            var cut = ctx.Render<NewAdventure>();
            Assert.Contains("Facile", cut.Markup);
            Assert.Contains("Moyen", cut.Markup);
            Assert.Contains("Difficile", cut.Markup);
        }

        [Fact]
        public void ClickingLevelNavigatesToGenerator()
        {
            using var ctx = new Bunit.BunitContext();
            var nav = ctx.Services.GetRequiredService<NavigationManager>();
            var cut = ctx.Render<NewAdventure>();

            var btn1 = cut.Find(".level-1");
            btn1.Click();
            Assert.Contains("/donjonGenerator?level=1", nav.Uri);

            var btn2 = cut.Find(".level-2");
            btn2.Click();
            Assert.Contains("/donjonGenerator?level=2", nav.Uri);

            var btn3 = cut.Find(".level-3");
            btn3.Click();
            Assert.Contains("/donjonGenerator?level=3", nav.Uri);
        }
    }
}
