using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;

namespace BlazorGame.Tests.ServicesTests
{
    public class RepositoryTests
    {
        [Fact]
        public async Task Add_List_GetById_Work()
        {
            // Summary: Test basique du Repository generic (Add, List, GetById).

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);

            var d = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "R" };

            // Act
            var added = await repo.AddAsync(d, CancellationToken.None);
            var list = await repo.ListAsync(CancellationToken.None);
            var byId = await repo.GetByIdAsync(d.Id, CancellationToken.None);

            // Assert
            Assert.Equal(added.Id, d.Id);
            Assert.Contains(list, x => x.Id == d.Id);
            Assert.NotNull(byId);
        }
    }
}
