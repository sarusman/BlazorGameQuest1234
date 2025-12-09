using System;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorGame.Client.Pages.Player.Aventure;

namespace BlazorGame.Tests.ClientTests.NewAdventureTests
{
    public class NewAdventureTests : Bunit.BunitContext
    {
        public NewAdventureTests()
        {
            this.JSInterop.Setup<string>("eval", _ => true).SetResult("testpseudo");
            // Ajoute l'authorization et un utilisateur authentifié
            this.Services.AddAuthorization();
            this.Services.AddSingleton<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
                new TestAuthenticationStateProvider("testuser")
            );
        }
        [Fact]
        public void RendersLevelButtons()
        {
            var cut = this.Render<NewAdventure>();
            Assert.Contains("Facile", cut.Markup);
            Assert.Contains("Moyen", cut.Markup);
            Assert.Contains("Difficile", cut.Markup);
        }

        [Fact]
        public void ClickingLevelNavigatesToGenerator()
        {
            var nav = this.Services.GetRequiredService<NavigationManager>();
            var cut = this.Render<NewAdventure>();

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
