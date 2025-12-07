using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.GameServiceTests.ServicesTests
{
    public class AdminServiceTests
    {
        [Fact]
        public async Task GetJoueursAsync_ReturnsJoueurs()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            var joueurs = new List<Joueur> { new Joueur { Id = Guid.NewGuid(), Pseudo = "A" } };
            var repo = new Mock<Repository<Joueur>>(db);
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(joueurs);
            var partieRepo = new Mock<Repository<Partie>>(db);
            var scoresSvc = new Mock<ScoresService>(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc.Object);
            var result = await svc.GetJoueursAsync(CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task SetJoueurActifAsync_UpdatesActif()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            var joueur = new Joueur { Id = Guid.NewGuid(), Actif = false };
            var repo = new Mock<Repository<Joueur>>(db);
            repo.Setup(r => r.GetByIdAsync(joueur.Id, It.IsAny<CancellationToken>())).ReturnsAsync(joueur);
            repo.Setup(r => r.UpdateAsync(joueur, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var partieRepo = new Mock<Repository<Partie>>(db);
            var scoresSvc = new Mock<ScoresService>(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc.Object);
            var result = await svc.SetJoueurActifAsync(joueur.Id, true, CancellationToken.None);
            Assert.NotNull(result);
            Assert.True(result.Actif);
        }

        [Fact]
        public async Task GetScoresAsync_ReturnsScores()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            // Arrange
            var score = new SharedModels.Domain.Scores.Score { Id = Guid.NewGuid(), Valeur = 10, EnregistreLe = DateTime.UtcNow };
            db.Scores.Add(score);
            db.SaveChanges();
            var repo = new Mock<Repository<Joueur>>(db);
            var partieRepo = new Mock<Repository<Partie>>(db);
            var scoresSvc = new ScoresService(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc);
            var result = await svc.GetScoresAsync(CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetLeaderboardAsync_ReturnsLeaderboard()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            // Arrange
            var joueurId = Guid.NewGuid();
            var joueur = new Joueur { Id = joueurId, Pseudo = "TestUser" };
            db.Joueurs.Add(joueur);
            var score = new SharedModels.Domain.Scores.Score { Id = Guid.NewGuid(), JoueurId = joueurId, Valeur = 10, EnregistreLe = DateTime.UtcNow };
            db.Scores.Add(score);
            db.SaveChanges();
            var repo = new Mock<Repository<Joueur>>(db);
            var partieRepo = new Mock<Repository<Partie>>(db);
            var scoresSvc = new ScoresService(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc);
            var result = await svc.GetLeaderboardAsync(CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetPartiesAsync_ReturnsParties()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            var parties = new List<Partie> { new Partie { Id = Guid.NewGuid() } };
            var partieRepo = new Mock<Repository<Partie>>(db);
            partieRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(parties);
            var repo = new Mock<Repository<Joueur>>(db);
            var scoresSvc = new Mock<ScoresService>(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc.Object);
            var result = await svc.GetPartiesAsync(CancellationToken.None);
            Assert.Single(result);
        }

        [Fact]
        public async Task ExportJoueursCsvAsync_ReturnsCsv()
        {
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new GameDbContext(opts);
            var joueurs = new List<Joueur> { new Joueur { Id = Guid.NewGuid(), Pseudo = "A", Actif = true } };
            var repo = new Mock<Repository<Joueur>>(db);
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(joueurs);
            var partieRepo = new Mock<Repository<Partie>>(db);
            var scoresSvc = new Mock<ScoresService>(db);
            var svc = new AdminService(repo.Object, partieRepo.Object, scoresSvc.Object);
            var result = await svc.ExportJoueursCsvAsync(CancellationToken.None);
            Assert.Contains(joueurs[0].Pseudo, result);
        }
    }
}
