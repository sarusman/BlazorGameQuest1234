using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ServicesTests
{
    public class DonjonServiceTests
    {
        [Fact]
        public async Task CreateAsync_CreatesDonjonWithRequestedNumberOfSalles()
        {
            // Summary: Crée un donjon via le service et vérifie le nombre de salles.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act
            var d = await svc.CreateAsync("MyDonjon", Difficulte.Facile, 2, 123, CancellationToken.None);
            var loaded = await svc.GetByIdAsync(d.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(d);
            Assert.Equal(2, d.Salles.Count);
            Assert.NotNull(loaded);
        }
    }
}
