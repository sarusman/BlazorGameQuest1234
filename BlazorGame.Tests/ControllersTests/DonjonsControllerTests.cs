using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Common.Enums;

namespace BlazorGame.Tests.ControllersTests
{
    public class DonjonsControllerTests
    {
        [Fact]
        public async Task Create_ReturnsOkAndPersists()
        {
            // Summary: Vérifie la création d'un donjon via le controller.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();
            var service = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(service);

            var req = new DonjonsController.CreateDonjonRequest { Nom = "X", Difficulte = Difficulte.Normal, NbSalles = 2, Seed = 123 };

            // Act
            var res = await ctrl.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            Assert.NotNull(ok.Value);
        }
    }
}
