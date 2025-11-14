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
    public class ScoresService_GetByDonjonTests
    {
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
    }
}
