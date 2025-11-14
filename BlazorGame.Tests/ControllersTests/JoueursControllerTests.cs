using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;

namespace BlazorGame.Tests.ControllersTests
{
    public class JoueursControllerTests
    {
        [Fact]
        public async Task RegisterAndLoginAndGetById_Works()
        {
            // Summary: Test basique d'inscription, login et récupération.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            var register = new JoueursController.RegisterRequest { Pseudo = "p1", Email = "a@b" };

            // Act - register
            var reg = await ctrl.Register(register, CancellationToken.None);
            var regOk = Assert.IsType<OkObjectResult>(reg.Result);
            var joueur = Assert.IsType<Joueur>(regOk.Value);

            // Act - login
            var login = new JoueursController.LoginRequest { Pseudo = "p1" };
            var log = await ctrl.Login(login, CancellationToken.None);
            var logOk = Assert.IsType<OkObjectResult>(log.Result);

            // Act - get by id
            var get = await ctrl.GetById(joueur.Id, CancellationToken.None);
            var getOk = Assert.IsType<OkObjectResult>(get.Result);

            // Assert
            Assert.Equal(joueur.Id, ((Joueur)getOk.Value!).Id);
        }
    }
}
