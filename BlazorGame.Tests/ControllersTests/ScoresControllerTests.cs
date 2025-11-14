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
    }
}
