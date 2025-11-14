using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;

namespace BlazorGame.Tests.ControllersTests
{
    public class ScoresControllerTests
    {
        [Fact]
        public async Task PostAndGetAll_And_ByDonjon_NotFound()
        {
            // Summary: Crée un score via le controller et vérifie GetAll et ByDonjon renvoie NotFound.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            var req = new ScoresController.ScoreRequest { JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 10 };

            // Act
            var post = await ctrl.Post(req, CancellationToken.None);
            var getAll = await ctrl.GetAll(CancellationToken.None);
            var byDonjon = await ctrl.ByDonjon(Guid.NewGuid(), CancellationToken.None);

            // Assert
            var okPost = Assert.IsType<OkObjectResult>(post.Result);
            Assert.NotNull(okPost.Value);

            var okGet = Assert.IsType<OkObjectResult>(getAll.Result);
            Assert.NotNull(okGet.Value);

            Assert.IsType<NotFoundResult>(byDonjon.Result);
        }

        [Fact]
        public async Task ByDonjon_ReturnsOk_WhenScoreExists()
        {
            // Summary: Vérifie que ByDonjon renvoie Ok si un score existe pour le donjon.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            var donjonId = Guid.NewGuid();
            var partie = new SharedModels.Domain.Gameplay.Partie { Id = Guid.NewGuid(), DonjonId = donjonId, JoueurId = Guid.NewGuid() };
            await db.Parties.AddAsync(partie, CancellationToken.None);
            var score = new SharedModels.Domain.Scores.Score { Id = Guid.NewGuid(), PartieId = partie.Id, JoueurId = partie.JoueurId, Valeur = 12, EnregistreLe = DateTime.UtcNow };
            await db.Scores.AddAsync(score, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act
            var byDonjon = await ctrl.ByDonjon(donjonId, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(byDonjon.Result);
            Assert.NotNull(ok.Value);
        }
    }
}


