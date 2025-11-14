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

        [Fact]
        public async Task DemarrerAsync_ReturnsNull_WhenDonjonMissingOrDeja()
        {
            // Summary: Vérifie que DemarrerAsync retourne null si le donjon est absent ou si une partie existe déjà.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act - donjon absent
            var resMissing = await svc.DemarrerAsync(joueurId, Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.Null(resMissing);

            // Arrange - create donjon and an existing partie
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D" };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            var partie = new Partie { Id = Guid.NewGuid(), JoueurId = joueurId, DonjonId = donjon.Id, EstTerminee = false };
            await db.Parties.AddAsync(partie, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act - déjà existante
            var resDeja = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);

            // Assert
            Assert.Null(resDeja);
        }

        [Fact]
        public async Task AppliquerChoixAsync_HandlesNullAndTerminalStates()
        {
            // Summary: Exercices les chemins où la partie/donjon/salle/choix manquent ou la partie est terminée.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new PartieService(db);

            // Act - no partie exists
            var noPart = await svc.AppliquerChoixAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
            Assert.Equal((0, false, true, (Guid?)null), noPart);

            // Arrange - create partie but mark finished
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D" };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            var p = new Partie { Id = Guid.NewGuid(), JoueurId = Guid.NewGuid(), DonjonId = donjon.Id, EstTerminee = true, ScoreFinal = 5 };
            await db.Parties.AddAsync(p, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            // Act - partie terminated
            var terminated = await svc.AppliquerChoixAsync(p.Id, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.Equal((p.ScoreFinal, false, true, (Guid?)null), terminated);
        }

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
            Assert.NotEqual(0, r1.score); // first applied
            Assert.Equal(r2.score, r1.score); // second application should not change score
        }
    }
}
