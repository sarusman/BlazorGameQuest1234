using System;
using System.Linq;
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

        [Fact]
        public async Task GetByIdAsync_IncludesSallesChoixEffets()
        {
            // Summary: Vérifie que GetByIdAsync retourne donjon avec salles et leurs choix/effets (Includes).

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 1, Description = "+1" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1" };
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new DonjonService(new Repository<Donjon>(db), new SalleService(), db);

            // Act
            var got = await svc.GetByIdAsync(donjon.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(got);
            Assert.NotNull(got!.Salles);
            Assert.NotEmpty(got.Salles);
            Assert.NotEmpty(got.Salles.First().ChoixProposes);
            Assert.NotEmpty(got.Salles.First().ChoixProposes.First().Effets);
        }

        [Fact]
        public async Task CreateAsync_UsesDefaultNbByDifficulty()
        {
            // Summary: Vérifie que CreateAsync utilise le nombre de salles par défaut selon la difficulté.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act
            var dEasy = await svc.CreateAsync("E", SharedModels.Domain.Common.Enums.Difficulte.Facile, 0, 1, CancellationToken.None);
            var dNormal = await svc.CreateAsync("N", SharedModels.Domain.Common.Enums.Difficulte.Normal, 0, 2, CancellationToken.None);
            var dHard = await svc.CreateAsync("H", SharedModels.Domain.Common.Enums.Difficulte.Difficile, 0, 3, CancellationToken.None);

            // Assert (NbParDifficulte: Facile=2, Normal=3, Difficile=5)
            Assert.Equal(2, dEasy.Salles.Count);
            Assert.Equal(3, dNormal.Salles.Count);
            Assert.Equal(5, dHard.Salles.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenDonjonDoesNotExist()
        {
            // Summary: Vérifie que GetByIdAsync retourne null quand le donjon n'existe pas.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new DonjonService(new Repository<Donjon>(db), new SalleService(), db);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await svc.GetByIdAsync(nonExistentId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_UsesDefaultName_WhenNameIsNullOrEmpty()
        {
            // Summary: Vérifie que CreateAsync utilise "Donjon" comme nom par défaut quand le nom est null ou vide.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act
            var d1 = await svc.CreateAsync(null!, Difficulte.Normal, 1, 1, CancellationToken.None);
            var d2 = await svc.CreateAsync("", Difficulte.Normal, 1, 2, CancellationToken.None);
            var d3 = await svc.CreateAsync("   ", Difficulte.Normal, 1, 3, CancellationToken.None);

            // Assert
            Assert.Equal("Donjon", d1.Nom);
            Assert.Equal("Donjon", d2.Nom);
            Assert.Equal("Donjon", d3.Nom);
        }

        [Fact]
        public async Task CreateAsync_GeneratesUniqueId()
        {
            // Summary: Vérifie que CreateAsync génère un Id unique pour chaque donjon.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act
            var d1 = await svc.CreateAsync("Donjon1", Difficulte.Normal, 1, 1, CancellationToken.None);
            var d2 = await svc.CreateAsync("Donjon2", Difficulte.Normal, 1, 2, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, d1.Id);
            Assert.NotEqual(Guid.Empty, d2.Id);
            Assert.NotEqual(d1.Id, d2.Id);
        }

        [Fact]
        public async Task CreateAsync_SetsCorrectProperties()
        {
            // Summary: Vérifie que CreateAsync définit correctement les propriétés du donjon (Nom, Difficulte, NbMaxSalles).

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act
            var d = await svc.CreateAsync("Mon Donjon", Difficulte.Difficile, 5, 42, CancellationToken.None);

            // Assert
            Assert.Equal("Mon Donjon", d.Nom);
            Assert.Equal(Difficulte.Difficile, d.Difficulte);
            Assert.Equal(5, d.NbMaxSalles);
            Assert.Equal(5, d.Salles.Count);
        }

        [Fact]
        public async Task CreateAsync_WithSameSeed_GeneratesSameSalles()
        {
            // Summary: Vérifie que CreateAsync avec le même seed génère les mêmes salles (reproductibilité).

            // Arrange
            var opts1 = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var opts2 = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db1 = new GameDbContext(opts1);
            await using var db2 = new GameDbContext(opts2);
            var repo1 = new Repository<Donjon>(db1);
            var repo2 = new Repository<Donjon>(db2);
            var salleSvc = new SalleService();
            var svc1 = new DonjonService(repo1, salleSvc, db1);
            var svc2 = new DonjonService(repo2, salleSvc, db2);
            const int seed = 12345;

            // Act
            var d1 = await svc1.CreateAsync("Donjon1", Difficulte.Normal, 3, seed, CancellationToken.None);
            var d2 = await svc2.CreateAsync("Donjon2", Difficulte.Normal, 3, seed, CancellationToken.None);

            // Assert
            Assert.Equal(3, d1.Salles.Count);
            Assert.Equal(3, d2.Salles.Count);
            // Les salles devraient avoir les mêmes titres avec le même seed
            var titres1 = d1.Salles.Select(s => s.Titre).OrderBy(t => t).ToList();
            var titres2 = d2.Salles.Select(s => s.Titre).OrderBy(t => t).ToList();
            Assert.Equal(titres1, titres2);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDonjonWithoutSalles_WhenDonjonHasNoSalles()
        {
            // Summary: Vérifie que GetByIdAsync retourne un donjon même s'il n'a pas de salles.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var donjon = new Donjon
            {
                Id = Guid.NewGuid(),
                Nom = "Donjon Vide",
                Difficulte = Difficulte.Normal,
                NbMaxSalles = 0,
                Salles = new List<Salle>()
            };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new DonjonService(new Repository<Donjon>(db), new SalleService(), db);

            // Act
            var result = await svc.GetByIdAsync(donjon.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(donjon.Id, result!.Id);
            Assert.Equal("Donjon Vide", result.Nom);
            Assert.NotNull(result.Salles);
            Assert.Empty(result.Salles);
        }

        [Fact]
        public async Task CreateAsync_RespectsNbSallesParameter_WhenGreaterThanZero()
        {
            // Summary: Vérifie que CreateAsync respecte le paramètre nbSalles quand il est > 0, même si différent de la difficulté.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act - Facile devrait normalement donner 2 salles, mais on force 7
            var d = await svc.CreateAsync("Test", Difficulte.Facile, 7, 999, CancellationToken.None);

            // Assert
            Assert.Equal(7, d.NbMaxSalles);
            Assert.Equal(7, d.Salles.Count);
        }

        [Fact]
        public async Task CreateAsync_UsesDefaultForInvalidDifficulty()
        {
            // Summary: Vérifie que CreateAsync utilise 3 salles pour une difficulté invalide (cas par défaut).

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var repo = new Repository<Donjon>(db);
            var salleSvc = new SalleService();
            var svc = new DonjonService(repo, salleSvc, db);

            // Act - Utilise une valeur non définie dans l'enum (cast explicite)
            var d = await svc.CreateAsync("Test", (Difficulte)999, 0, 1, CancellationToken.None);

            // Assert - Le switch devrait renvoyer 3 (cas par défaut)
            Assert.Equal(3, d.NbMaxSalles);
            Assert.Equal(3, d.Salles.Count);
        }
    }
}
