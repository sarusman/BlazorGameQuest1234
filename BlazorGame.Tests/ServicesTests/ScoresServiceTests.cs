using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;

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
    }
}
