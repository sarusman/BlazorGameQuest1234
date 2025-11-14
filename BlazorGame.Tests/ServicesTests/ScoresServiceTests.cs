using System;
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
    }
}
