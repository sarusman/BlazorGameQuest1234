using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Scores;

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

            var req = new ScoreRequest { JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 10 };

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

        [Fact]
        public async Task Post_CreatesScoreWithCorrectProperties()
        {
            // Summary: Vérifie que Post crée un score avec les propriétés correctes.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            var joueurId = Guid.NewGuid();
            var partieId = Guid.NewGuid();
            var req = new ScoreRequest { JoueurId = joueurId, PartieId = partieId, Valeur = 25 };

            // Act
            var post = await ctrl.Post(req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(post.Result);
            var score = Assert.IsType<Score>(ok.Value);
            Assert.Equal(joueurId, score.JoueurId);
            Assert.Equal(partieId, score.PartieId);
            Assert.Equal(25, score.Valeur);
            Assert.NotEqual(Guid.Empty, score.Id);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoScores()
        {
            // Summary: Vérifie que GetAll retourne une liste vide quand aucun score n'existe.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            // Act
            var getAll = await ctrl.GetAll(CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(getAll.Result);
            var list = Assert.IsType<List<Score>>(ok.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAll_ReturnsMultipleScores_WhenScoresExist()
        {
            // Summary: Vérifie que GetAll retourne tous les scores quand ils existent.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), 10, CancellationToken.None);
            await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), 20, CancellationToken.None);

            // Act
            var getAll = await ctrl.GetAll(CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(getAll.Result);
            var list = Assert.IsType<List<Score>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task Leaderboard_ReturnsTop10()
        {
            // Summary: Vérifie que Leaderboard retourne au maximum 10 scores.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            // Créer 15 scores
            for (int i = 0; i < 15; i++)
            {
                await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), 100 - i, CancellationToken.None);
            }

            // Act
            var leaderboard = await ctrl.Leaderboard(CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(leaderboard.Result);
            var board = Assert.IsType<List<object>>(ok.Value);
            Assert.True(board.Count <= 10);
        }

        [Fact]
        public async Task Leaderboard_ReturnsOrderedScores()
        {
            // Summary: Vérifie que Leaderboard retourne les scores triés par valeur décroissante.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var ctrl = new ScoresController(svc);

            var joueurId1 = Guid.NewGuid();
            var joueurId2 = Guid.NewGuid();
            var joueurId3 = Guid.NewGuid();
            db.Joueurs.Add(new SharedModels.Domain.Users.Joueur { Id = joueurId1, Pseudo = "User1" });
            db.Joueurs.Add(new SharedModels.Domain.Users.Joueur { Id = joueurId2, Pseudo = "User2" });
            db.Joueurs.Add(new SharedModels.Domain.Users.Joueur { Id = joueurId3, Pseudo = "User3" });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            await svc.CreateAsync(joueurId1, Guid.NewGuid(), 50, CancellationToken.None);
            await svc.CreateAsync(joueurId2, Guid.NewGuid(), 100, CancellationToken.None);
            await svc.CreateAsync(joueurId3, Guid.NewGuid(), 75, CancellationToken.None);

            // Act
            var leaderboard = await ctrl.Leaderboard(CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(leaderboard.Result);
            var board = Assert.IsType<List<object>>(ok.Value);
            Assert.True(board.Count >= 3);
        }
    }
}


