using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Scores;

namespace BlazorGame.Tests.ServicesTests
{
    public class PartieServiceTests
    {
        [Fact]
        public async Task DemarrerAndAppliquerChoix_Works_EndToEnd()
        {
            // Summary: Démarre une partie et applique un choix simple, vérifie score et fin de partie.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            // create a donjon with one salle and a choix
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Test", Description = "Desc" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "OK", Type = SharedModels.Domain.Common.Enums.TypeChoix.Ignorer };
            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 5, Description = "+5" };
            choix.Effets.Add(effet);
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act - démarrer
            var p = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);

            // Assert - démarrer
            Assert.NotNull(p);

            // Act - appliquer choix
            var res = await svc.AppliquerChoixAsync(p.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert - appliquer choix
            Assert.True(res.fini || res.nextSalleId == null);
            Assert.Equal(p.ScoreFinal, res.score);
        }
    }
}
