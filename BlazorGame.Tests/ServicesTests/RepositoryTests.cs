using System;
using System.Linq;
using System.Threading;
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

        [Fact]
        public async Task GetById_ReturnsNull_WhenMissing()
        {
            // Summary: Vérifie que GetByIdAsync retourne null si l'entité est absente.

            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);

            var notFound = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.Null(notFound);
        }

        [Fact]
        public async Task AddAsync_PersistsEntityInDatabase()
        {
            // Summary: Vérifie que AddAsync sauvegarde bien l'entité en base de données.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var donjon = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "Test" };

            // Act
            await repo.AddAsync(donjon, CancellationToken.None);

            // Assert - Vérifier que l'entité est bien en base
            var found = await db.Donjons.FirstOrDefaultAsync(d => d.Id == donjon.Id, CancellationToken.None);
            Assert.NotNull(found);
            Assert.Equal("Test", found!.Nom);
        }

        [Fact]
        public async Task ListAsync_ReturnsMultipleEntities()
        {
            // Summary: Vérifie que ListAsync retourne plusieurs entités quand elles existent.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);

            var d1 = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "D1" };
            var d2 = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "D2" };
            await repo.AddAsync(d1, CancellationToken.None);
            await repo.AddAsync(d2, CancellationToken.None);

            // Act
            var list = await repo.ListAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Contains(list, d => d.Id == d1.Id);
            Assert.Contains(list, d => d.Id == d2.Id);
        }

        [Fact]
        public async Task Repository_WorksWithDifferentEntityTypes()
        {
            // Summary: Vérifie que Repository fonctionne avec différents types d'entités.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repoDonjon = new Repository<SharedModels.Domain.Gameplay.Donjon>(db);
            var repoJoueur = new Repository<SharedModels.Domain.Users.Joueur>(db);

            var donjon = new SharedModels.Domain.Gameplay.Donjon { Id = Guid.NewGuid(), Nom = "D" };
            var joueur = new SharedModels.Domain.Users.Joueur { Id = Guid.NewGuid(), Pseudo = "J" };

            // Act
            await repoDonjon.AddAsync(donjon, CancellationToken.None);
            await repoJoueur.AddAsync(joueur, CancellationToken.None);

            // Assert
            var foundDonjon = await repoDonjon.GetByIdAsync(donjon.Id, CancellationToken.None);
            var foundJoueur = await repoJoueur.GetByIdAsync(joueur.Id, CancellationToken.None);
            Assert.NotNull(foundDonjon);
            Assert.NotNull(foundJoueur);
            Assert.Equal("D", foundDonjon!.Nom);
            Assert.Equal("J", foundJoueur!.Pseudo);
        }
    }
}

