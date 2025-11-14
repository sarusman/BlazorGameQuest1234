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
    public class PartieServiceEdgeTests
    {
        [Fact]
        public async Task AppliquerChoix_MortInstantanee_EndsWithMort()
        {
            // Summary: Vérifie qu'un effet MortInstantanee termine la partie et marque mort.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.MortInstantanee, Description = "Boom" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Mort" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S" };
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            var p = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(p);

            // Act
            var r = await svc.AppliquerChoixAsync(p.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            Assert.True(r.mort);
            Assert.True(r.fini);
        }

        [Fact]
        public async Task AppliquerChoix_RepeatedChoice_ReturnsNoChange()
        {
            // Summary: Vérifie qu'appliquer deux fois le même choix n'altère pas la partie la seconde fois.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 1, Description = "+1" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "C" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S" };
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            var p = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(p);

            // Act - first apply
            var r1 = await svc.AppliquerChoixAsync(p.Id, salle.Id, choix.Id, CancellationToken.None);
            // Act - second apply (same choix)
            var r2 = await svc.AppliquerChoixAsync(p.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            Assert.NotEqual(r1.score, 0); // first applied
            Assert.Equal(r2.score, r1.score); // second application should not change score
        }
    }
}
