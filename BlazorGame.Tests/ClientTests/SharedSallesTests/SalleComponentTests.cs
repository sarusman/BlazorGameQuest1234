using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
using BlazorGame.Client.Shared;

namespace BlazorGame.Tests.ClientTests.SharedSallesTests
{
    public class SalleComponentTests
    {
        [Fact]
        public void RendersSceneAndChoices()
        {
            using var ctx = new Bunit.BunitContext();
            var cut = ctx.Render<SalleComponent>(parameters => parameters
                .Add(p => p.SceneDescription, "Un dragon bloque le passage.")
                .Add(p => p.CombatChoice, "Combattre")
                .Add(p => p.FuiteChoice, "Fuir")
                .Add(p => p.FouilleChoice, "Fouiller")
            );
            Assert.Contains("Un dragon bloque le passage.", cut.Markup);
            Assert.Contains("Combattre", cut.Markup);
            Assert.Contains("Fuir", cut.Markup);
            Assert.Contains("Fouiller", cut.Markup);
        }

        [Fact]
        public async Task ClickingCombatShowsResultAndInvokesCallback()
        {
            using var ctx = new Bunit.BunitContext();
            string? selected = null;
            var cut = ctx.Render<SalleComponent>(parameters => parameters
                .Add(p => p.SceneDescription, "Un dragon bloque le passage.")
                .Add(p => p.CombatChoice, "Combattre")
                .Add(p => p.FuiteChoice, "Fuir")
                .Add(p => p.FouilleChoice, "Fouiller")
                .Add(p => p.OnChoiceSelected, EventCallback.Factory.Create<string>(this, s => selected = s))
            );
            var btn = cut.Find(".btn-danger");
            await btn.ClickAsync();
            Assert.Contains("Vous engagez le combat", cut.Markup);
            Assert.Equal("combat", selected);
        }

        [Fact]
        public async Task ClickingFuiteShowsResultAndInvokesCallback()
        {
            using var ctx = new Bunit.BunitContext();
            string? selected = null;
            var cut = ctx.Render<SalleComponent>(parameters => parameters
                .Add(p => p.SceneDescription, "Un dragon bloque le passage.")
                .Add(p => p.CombatChoice, "Combattre")
                .Add(p => p.FuiteChoice, "Fuir")
                .Add(p => p.FouilleChoice, "Fouiller")
                .Add(p => p.OnChoiceSelected, EventCallback.Factory.Create<string>(this, s => selected = s))
            );
            var btn = cut.Find(".btn-warning");
            await btn.ClickAsync();
            Assert.Contains("Vous prenez la fuite", cut.Markup);
            Assert.Equal("fuite", selected);
        }

        [Fact]
        public async Task ClickingFouilleShowsResultAndInvokesCallback()
        {
            using var ctx = new Bunit.BunitContext();
            string? selected = null;
            var cut = ctx.Render<SalleComponent>(parameters => parameters
                .Add(p => p.SceneDescription, "Un dragon bloque le passage.")
                .Add(p => p.CombatChoice, "Combattre")
                .Add(p => p.FuiteChoice, "Fuir")
                .Add(p => p.FouilleChoice, "Fouiller")
                .Add(p => p.OnChoiceSelected, EventCallback.Factory.Create<string>(this, s => selected = s))
            );
            var btn = cut.Find(".btn-info");
            await btn.ClickAsync();
            Assert.Contains("Vous fouillez les environs", cut.Markup);
            Assert.Equal("fouille", selected);
        }
    }
}
