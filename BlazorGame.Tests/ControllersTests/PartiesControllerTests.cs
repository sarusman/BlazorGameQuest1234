using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ControllersTests
{
    public class PartiesControllerTests
    {
        [Fact]
        public async Task DemarrerAndChoisirEndpoints_Work()
        {
            // Summary: Test basique des endpoints Demarrer et Choisir via le controller.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1", Description = "d" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            choix.Effets.Add(new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 1, Description = "+1" });
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);

            var start = new PartiesController.StartPartieRequest { JoueurId = Guid.NewGuid(), DonjonId = donjon.Id };

            // Act - Demarrer
            var res = await ctrl.Demarrer(start, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var partie = Assert.IsType<Partie>(ok.Value);

            // Act - Choisir
            var chooseReq = new PartiesController.ChoisirRequest { SalleId = salle.Id, ChoixId = choix.Id };
            var chooseRes = await ctrl.Choisir(partie.Id, chooseReq, CancellationToken.None);

            // Assert choose
            var okChoose = Assert.IsType<OkObjectResult>(chooseRes.Result);
            Assert.NotNull(okChoose.Value);
        }
    }
}
