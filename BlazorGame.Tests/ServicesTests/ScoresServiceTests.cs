using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Scores;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ServicesTests
{
    public class ScoresServiceTests
    {
        [Fact]
        public async Task CreateAndGetAll_Workflow_Works()
        {
            // Summary: Crée un score en mémoire et vérifie qu'il est retourné par GetAll.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            var joueurId = Guid.NewGuid();
            var partieId = Guid.NewGuid();

            // Act
            var created = await svc.CreateAsync(joueurId, partieId, 42, CancellationToken.None);
            var all = await svc.GetAllAsync(CancellationToken.None);
            var board = await svc.GetLeaderboardAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(created);
            Assert.Contains(all, x => x.Id == created.Id);
            Assert.NotEmpty(board);
        }

        [Fact]
        public async Task GetByDonjonAsync_ReturnsLatestScoreForDonjon()
        {
            // Summary: Vérifie que GetByDonjonAsync retourne le score le plus récent lié à un donjon.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D" };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);

            var partie = new Partie { Id = Guid.NewGuid(), DonjonId = donjon.Id, JoueurId = Guid.NewGuid() };
            await db.Parties.AddAsync(partie, CancellationToken.None);

            var old = new Score { Id = Guid.NewGuid(), PartieId = partie.Id, JoueurId = partie.JoueurId, Valeur = 5, EnregistreLe = DateTime.UtcNow.AddMinutes(-5) };
            var recent = new Score { Id = Guid.NewGuid(), PartieId = partie.Id, JoueurId = partie.JoueurId, Valeur = 20, EnregistreLe = DateTime.UtcNow };
            await db.Scores.AddAsync(old, CancellationToken.None);
            await db.Scores.AddAsync(recent, CancellationToken.None);

            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new ScoresService(db);

            // Act
            var res = await svc.GetByDonjonAsync(donjon.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(res);
            Assert.Equal(20, res!.Valeur);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoScores()
        {
            // Summary: Vérifie que GetAll retourne une liste vide si aucun score.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            // Act
            var all = await svc.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(all);
            Assert.Empty(all);
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsTopOrdered()
        {
            // Summary: Vérifie que GetLeaderboard renvoie les meilleurs scores ordonnés.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            var j1 = Guid.NewGuid();
            var p1 = Guid.NewGuid();
            await db.Scores.AddAsync(new Score { Id = Guid.NewGuid(), JoueurId = j1, PartieId = p1, Valeur = 100, EnregistreLe = DateTime.UtcNow }, CancellationToken.None);
            await db.Scores.AddAsync(new Score { Id = Guid.NewGuid(), JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 50, EnregistreLe = DateTime.UtcNow }, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act
            var board = await svc.GetLeaderboardAsync(CancellationToken.None);

            // Assert
            Assert.NotEmpty(board);
        }

        [Fact]
        public async Task CreateAsync_CreatesScoreWithCorrectProperties()
        {
            // Summary: Vérifie que CreateAsync crée un score avec les propriétés correctes.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var joueurId = Guid.NewGuid();
            var partieId = Guid.NewGuid();
            const int valeur = 42;

            // Act
            var score = await svc.CreateAsync(joueurId, partieId, valeur, CancellationToken.None);

            // Assert
            Assert.NotNull(score);
            Assert.NotEqual(Guid.Empty, score.Id);
            Assert.Equal(joueurId, score.JoueurId);
            Assert.Equal(partieId, score.PartieId);
            Assert.Equal(valeur, score.Valeur);
            Assert.True(score.EnregistreLe <= DateTime.UtcNow);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsScoresInDescendingOrder()
        {
            // Summary: Vérifie que GetAllAsync retourne les scores dans l'ordre décroissant par date.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            var score1 = new Score { Id = Guid.NewGuid(), JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 10, EnregistreLe = DateTime.UtcNow.AddMinutes(-10) };
            var score2 = new Score { Id = Guid.NewGuid(), JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 20, EnregistreLe = DateTime.UtcNow.AddMinutes(-5) };
            var score3 = new Score { Id = Guid.NewGuid(), JoueurId = Guid.NewGuid(), PartieId = Guid.NewGuid(), Valeur = 30, EnregistreLe = DateTime.UtcNow };
            await db.Scores.AddAsync(score1, CancellationToken.None);
            await db.Scores.AddAsync(score2, CancellationToken.None);
            await db.Scores.AddAsync(score3, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act
            var all = await svc.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.Equal(3, all.Count);
            Assert.Equal(score3.Id, all[0].Id); // Plus récent en premier
            Assert.Equal(score2.Id, all[1].Id);
            Assert.Equal(score1.Id, all[2].Id); // Plus ancien en dernier
        }

        [Fact]
        public async Task GetLeaderboardAsync_ReturnsTop10()
        {
            // Summary: Vérifie que GetLeaderboardAsync retourne au maximum 10 scores.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            // Créer 15 scores
            for (int i = 0; i < 15; i++)
            {
                await db.Scores.AddAsync(new Score
                {
                    Id = Guid.NewGuid(),
                    JoueurId = Guid.NewGuid(),
                    PartieId = Guid.NewGuid(),
                    Valeur = 100 - i,
                    EnregistreLe = DateTime.UtcNow.AddMinutes(-i)
                }, CancellationToken.None);
            }
            await db.SaveChangesAsync(CancellationToken.None);

            // Act
            var board = await svc.GetLeaderboardAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(board);
            Assert.True(board.Count <= 10);
        }

        

        [Fact]
        public async Task GetByDonjonAsync_ReturnsNull_WhenNoScoreExists()
        {
            // Summary: Vérifie que GetByDonjonAsync retourne null quand aucun score n'existe pour le donjon.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);
            var donjonId = Guid.NewGuid();

            // Act
            var result = await svc.GetByDonjonAsync(donjonId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByDonjonAsync_ReturnsMostRecentScore_WhenMultipleScoresExist()
        {
            // Summary: Vérifie que GetByDonjonAsync retourne le score le plus récent quand plusieurs scores existent.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new ScoresService(db);

            var donjonId = Guid.NewGuid();
            var partie1 = new Partie { Id = Guid.NewGuid(), DonjonId = donjonId, JoueurId = Guid.NewGuid() };
            var partie2 = new Partie { Id = Guid.NewGuid(), DonjonId = donjonId, JoueurId = Guid.NewGuid() };
            await db.Parties.AddAsync(partie1, CancellationToken.None);
            await db.Parties.AddAsync(partie2, CancellationToken.None);

            var oldScore = new Score { Id = Guid.NewGuid(), PartieId = partie1.Id, JoueurId = partie1.JoueurId, Valeur = 10, EnregistreLe = DateTime.UtcNow.AddHours(-2) };
            var recentScore = new Score { Id = Guid.NewGuid(), PartieId = partie2.Id, JoueurId = partie2.JoueurId, Valeur = 50, EnregistreLe = DateTime.UtcNow };
            await db.Scores.AddAsync(oldScore, CancellationToken.None);
            await db.Scores.AddAsync(recentScore, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await svc.GetByDonjonAsync(donjonId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recentScore.Id, result!.Id);
            Assert.Equal(50, result.Valeur);
        }
    }
}
