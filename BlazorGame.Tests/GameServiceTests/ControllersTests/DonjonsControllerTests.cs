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
using SharedModels.Domain.Sealed;

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

            var req = new CreateDonjonRequest { Nom = "X", Difficulte = Difficulte.Normal, NbSalles = 2, Seed = 123 };

            // Act
            var res = await ctrl.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            // Summary: Vérifie que GetById renvoie Ok si le donjon existe.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();

            var donjon = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "DTest" };
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

        [Fact]
        public async Task Create_CreatesDonjonWithCorrectProperties()
        {
            // Summary: Vérifie que Create crée un donjon avec les propriétés correctes.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();
            var service = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(service);

            var req = new CreateDonjonRequest { Nom = "Mon Donjon", Difficulte = Difficulte.Difficile, NbSalles = 5, Seed = 42 };

            // Act
            var res = await ctrl.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var donjon = Assert.IsType<SharedModels.Domain.Gameplay.Donjon>(ok.Value);
            Assert.Equal("Mon Donjon", donjon.Nom);
            Assert.Equal(Difficulte.Difficile, donjon.Difficulte);
            Assert.Equal(5, donjon.NbMaxSalles);
            Assert.Equal(5, donjon.Salles.Count);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDonjonDoesNotExist()
        {
            // Summary: Vérifie que GetById retourne NotFound quand le donjon n'existe pas.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(svc);

            // Act
            var res = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public async Task Create_WithNullNom_UsesDefaultName()
        {
            // Summary: Vérifie que Create utilise un nom par défaut quand le nom est null.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();
            var service = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(service);

            var req = new CreateDonjonRequest { Nom = null, Difficulte = Difficulte.Normal, NbSalles = 2, Seed = 123 };

            // Act
            var res = await ctrl.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var donjon = Assert.IsType<SharedModels.Domain.Gameplay.Donjon>(ok.Value);
            Assert.Equal("Donjon", donjon.Nom);
        }

        [Fact]
        public async Task Create_WithZeroNbSalles_UsesDefaultByDifficulty()
        {
            // Summary: Vérifie que Create utilise le nombre de salles par défaut selon la difficulté quand nbSalles est 0.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var salleSvc = new SalleService();
            var service = new DonjonService(repo, salleSvc, db);
            var ctrl = new DonjonsController(service);

            var req = new CreateDonjonRequest { Nom = "Test", Difficulte = Difficulte.Facile, NbSalles = 0, Seed = 1 };

            // Act
            var res = await ctrl.Create(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var donjon = Assert.IsType<SharedModels.Domain.Gameplay.Donjon>(ok.Value);
            Assert.Equal(2, donjon.Salles.Count); // Facile = 2 salles par défaut
        }
    }
}
