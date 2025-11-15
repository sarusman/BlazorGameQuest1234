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

        [Fact]
        public async Task Login_NotFound_Returns404_And_GetById_NotFound()
        {
            // Summary: Vérifie que Login et GetById retournent NotFound quand l'entité n'existe pas.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            // Act - login missing
            var login = new JoueursController.LoginRequest { Pseudo = "nope" };
            var log = await ctrl.Login(login, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundObjectResult>(log.Result);

            // Act - get by id missing
            var get = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);
            Assert.IsType<NotFoundResult>(get.Result);
        }

        [Fact]
        public async Task Register_CreatesJoueurWithCorrectProperties()
        {
            // Summary: Vérifie que Register crée un joueur avec les propriétés correctes.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            var register = new JoueursController.RegisterRequest { Pseudo = "testuser", Email = "test@example.com" };

            // Act
            var reg = await ctrl.Register(register, CancellationToken.None);

            // Assert
            var regOk = Assert.IsType<OkObjectResult>(reg.Result);
            var joueur = Assert.IsType<Joueur>(regOk.Value);
            Assert.Equal("testuser", joueur.Pseudo);
            Assert.NotEqual(Guid.Empty, joueur.Id);
        }

        [Fact]
        public async Task Register_GeneratesUniqueId()
        {
            // Summary: Vérifie que Register génère un Id unique pour chaque joueur.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            var register1 = new JoueursController.RegisterRequest { Pseudo = "user1", Email = "user1@test.com" };
            var register2 = new JoueursController.RegisterRequest { Pseudo = "user2", Email = "user2@test.com" };

            // Act
            var reg1 = await ctrl.Register(register1, CancellationToken.None);
            var reg2 = await ctrl.Register(register2, CancellationToken.None);

            // Assert
            var joueur1 = Assert.IsType<Joueur>(((OkObjectResult)reg1.Result!).Value);
            var joueur2 = Assert.IsType<Joueur>(((OkObjectResult)reg2.Result!).Value);
            Assert.NotEqual(joueur1.Id, joueur2.Id);
        }

        [Fact]
        public async Task Login_ReturnsCorrectJoueur_WhenMultipleJoueursExist()
        {
            // Summary: Vérifie que Login retourne le bon joueur quand plusieurs joueurs existent.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            var register1 = new JoueursController.RegisterRequest { Pseudo = "alice", Email = "alice@test.com" };
            var register2 = new JoueursController.RegisterRequest { Pseudo = "bob", Email = "bob@test.com" };
            await ctrl.Register(register1, CancellationToken.None);
            await ctrl.Register(register2, CancellationToken.None);

            // Act
            var login = new JoueursController.LoginRequest { Pseudo = "alice" };
            var log = await ctrl.Login(login, CancellationToken.None);

            // Assert
            var logOk = Assert.IsType<OkObjectResult>(log.Result);
            var joueur = Assert.IsType<Joueur>(logOk.Value);
            Assert.Equal("alice", joueur.Pseudo);
        }

        [Fact]
        public async Task GetById_ReturnsCorrectJoueur()
        {
            // Summary: Vérifie que GetById retourne le bon joueur.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var ctrl = new JoueursController(repo);

            var register = new JoueursController.RegisterRequest { Pseudo = "test", Email = "test@test.com" };
            var reg = await ctrl.Register(register, CancellationToken.None);
            var joueur = Assert.IsType<Joueur>(((OkObjectResult)reg.Result!).Value);

            // Act
            var get = await ctrl.GetById(joueur.Id, CancellationToken.None);

            // Assert
            var getOk = Assert.IsType<OkObjectResult>(get.Result);
            var retrieved = Assert.IsType<Joueur>(getOk.Value);
            Assert.Equal(joueur.Id, retrieved.Id);
            Assert.Equal("test", retrieved.Pseudo);
        }
    }
}
