using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.GameServiceTests.ControllersTests
{
    public class AdminControllerTests
    {
        [Fact]
        public async Task GetJoueurs_ReturnsJoueurs()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            db.Joueurs.Add(new Joueur { Id = Guid.NewGuid(), Pseudo = "A" });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.GetJoueurs(CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
            Assert.Single((System.Collections.IEnumerable)ok.Value);
        }

        [Fact]
        public async Task GetScores_ReturnsScores()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.GetScores(CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsLeaderboard()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.GetLeaderboard(CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetParties_ReturnsParties()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            db.Parties.Add(new Partie { Id = Guid.NewGuid() });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.GetParties(CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
            Assert.Single((System.Collections.IEnumerable)ok.Value);
        }

        [Fact]
        public async Task ExportJoueurs_ReturnsCsvFile()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            db.Joueurs.Add(new Joueur { Id = Guid.NewGuid(), Pseudo = "A", Actif = true });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.ExportJoueurs(CancellationToken.None);
            var file = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", file.ContentType);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAdminFound()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            var admin = new Joueur { Id = Guid.NewGuid(), Pseudo = "admin1234", Admin = true, Actif = true };
            db.Joueurs.Add(admin);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var req = new LoginRequest { Pseudo = "admin1234" };
            var result = await ctrl.Login(req, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
            Assert.Equal(admin.Id, ((Joueur)ok.Value).Id);
        }
        [Fact]
        public async Task SetJoueurActif_UpdatesStatus_ShouldWork_Unique()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var db = new GameDbContext(opts);
            var joueur = new Joueur { Id = Guid.NewGuid(), Pseudo = "admin", Actif = false };
            db.Joueurs.Add(joueur);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            var repo = new Repository<Joueur>(db);
            var partieRepo = new Repository<Partie>(db);
            var scoresService = new BlazorGame.GameService.Services.ScoresService(db);
            var ctrl = new AdminController(repo, partieRepo, scoresService);
            var result = await ctrl.SetJoueurActif(joueur.Id, true, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var updated = Assert.IsType<Joueur>(ok.Value);
            Assert.True(updated.Actif);
        }
    }
}
