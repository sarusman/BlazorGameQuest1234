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
    public class DonjonsController_GetByIdTests
    {
        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            // Summary: Vérifie que GetById renvoie Ok si le donjon existe.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "DTest" };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(svc);

            // Act
            var res = await ctrl.GetById(donjon.Id, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            Assert.NotNull(ok.Value);
        }
    }
}
