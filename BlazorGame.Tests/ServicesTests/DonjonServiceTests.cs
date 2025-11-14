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
    }
}
