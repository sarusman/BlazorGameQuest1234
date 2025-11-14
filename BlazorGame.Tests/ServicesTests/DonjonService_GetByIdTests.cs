using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ServicesTests
{
    public class DonjonService_GetByIdTests
    {
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
    }
}
