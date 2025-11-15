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

        [Fact]
        public async Task DemarrerAsync_CreatesPartieWithCorrectProperties()
        {
            // Summary: Vérifie que DemarrerAsync crée une partie avec les propriétés correctes (Id, JoueurId, DonjonId, ScoreFinal, EstTerminee).

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(partie);
            Assert.NotEqual(Guid.Empty, partie!.Id);
            Assert.Equal(joueurId, partie.JoueurId);
            Assert.Equal(donjon.Id, partie.DonjonId);
            Assert.Equal(10, partie.ScoreFinal);
            Assert.False(partie.EstTerminee);
            Assert.True(partie.DemarreeLe <= DateTime.UtcNow);
        }

        [Fact]
        public async Task DemarrerAsync_CreatesInitialScore()
        {
            // Summary: Vérifie que DemarrerAsync crée un score initial de 10.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(partie);
            var score = await db.Scores.FirstOrDefaultAsync(s => s.PartieId == partie!.Id, CancellationToken.None);
            Assert.NotNull(score);
            Assert.Equal(10, score!.Valeur);
            Assert.Equal(joueurId, score.JoueurId);
            Assert.Equal(partie.Id, score.PartieId);
        }

        [Fact]
        public async Task DemarrerAsync_ReturnsNull_WhenDonjonHasNoSalles()
        {
            // Summary: Vérifie que DemarrerAsync retourne null quand le donjon n'a pas de salles.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon Vide", Salles = new System.Collections.Generic.List<Salle>() };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);

            // Assert
            Assert.Null(partie);
        }

        [Fact]
        public async Task DemarrerAsync_GeneratesUniqueId()
        {
            // Summary: Vérifie que DemarrerAsync génère un Id unique pour chaque partie.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle1 = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var salle2 = new Salle { Id = Guid.NewGuid(), Titre = "Salle2" };
            var donjon1 = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle1 } };
            var donjon2 = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon2", Salles = new System.Collections.Generic.List<Salle> { salle2 } };
            await db.Donjons.AddAsync(donjon1, CancellationToken.None);
            await db.Donjons.AddAsync(donjon2, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();

            // Act
            var partie1 = await svc.DemarrerAsync(joueurId, donjon1.Id, CancellationToken.None);
            var partie2 = await svc.DemarrerAsync(joueurId, donjon2.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(partie1);
            Assert.NotNull(partie2);
            Assert.NotEqual(Guid.Empty, partie1!.Id);
            Assert.NotEqual(Guid.Empty, partie2!.Id);
            Assert.NotEqual(partie1.Id, partie2.Id);
        }

        [Fact]
        public async Task AppliquerChoixAsync_UpdatesScore_WithGainPoints()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync met à jour le score correctement avec un gain de points.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var effet = new Effet { Id = Guid.NewGuid(), Type = TypeEffet.GainPoints, Valeur = 15, Description = "+15" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Gagner" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);
            var scoreInitial = partie!.ScoreFinal; // 10

            // Act
            var result = await svc.AppliquerChoixAsync(partie.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            Assert.Equal(scoreInitial + 15, result.score); // 10 + 15 = 25
            Assert.False(result.mort);
        }

        [Fact]
        public async Task AppliquerChoixAsync_UpdatesScore_WithPertePoints()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync met à jour le score correctement avec une perte de points.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var effet = new Effet { Id = Guid.NewGuid(), Type = TypeEffet.PertePoints, Valeur = -5, Description = "-5" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Perdre" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);
            var scoreInitial = partie!.ScoreFinal; // 10

            // Act
            var result = await svc.AppliquerChoixAsync(partie.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            Assert.Equal(scoreInitial - 5, result.score); // 10 - 5 = 5
            Assert.False(result.mort);
        }

        [Fact]
        public async Task AppliquerChoixAsync_ReturnsNull_WhenSalleDoesNotExist()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync retourne le score actuel quand la salle n'existe pas.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);
            var scoreInitial = partie!.ScoreFinal;

            // Act
            var result = await svc.AppliquerChoixAsync(partie.Id, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.Equal(scoreInitial, result.score);
            Assert.False(result.mort);
            Assert.False(result.fini);
        }

        [Fact]
        public async Task AppliquerChoixAsync_ReturnsNull_WhenChoixDoesNotExist()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync retourne le score actuel quand le choix n'existe pas.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);
            var scoreInitial = partie!.ScoreFinal;

            // Act
            var result = await svc.AppliquerChoixAsync(partie.Id, salle.Id, Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.Equal(scoreInitial, result.score);
            Assert.False(result.mort);
            Assert.False(result.fini);
        }

        [Fact]
        public async Task AppliquerChoixAsync_FinishesPartie_WhenAllSallesVisited()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync termine la partie quand toutes les salles sont visitées.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle1 = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            var salle2 = new Salle { Id = Guid.NewGuid(), Titre = "Salle2" };
            var choix1 = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            var choix2 = new Choix { Id = Guid.NewGuid(), Libelle = "C2" };
            salle1.ChoixProposes.Add(choix1);
            salle2.ChoixProposes.Add(choix2);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle1, salle2 } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);

            // Act - première salle
            var result1 = await svc.AppliquerChoixAsync(partie!.Id, salle1.Id, choix1.Id, CancellationToken.None);
            Assert.False(result1.fini);

            // Act - dernière salle
            var result2 = await svc.AppliquerChoixAsync(partie.Id, salle2.Id, choix2.Id, CancellationToken.None);

            // Assert
            Assert.True(result2.fini);
            var partieFinale = await db.Parties.FirstOrDefaultAsync(p => p.Id == partie.Id, CancellationToken.None);
            Assert.NotNull(partieFinale);
            Assert.True(partieFinale!.EstTerminee);
        }

        [Fact]
        public async Task AppliquerChoixAsync_CreatesEtape()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync crée une étape de partie.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);

            // Act
            await svc.AppliquerChoixAsync(partie!.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            var etapes = await db.Etapes.Where(e => e.PartieId == partie.Id).ToListAsync(CancellationToken.None);
            Assert.Single(etapes);
            Assert.Equal(salle.Id, etapes[0].SalleId);
            Assert.Equal(choix.Id, etapes[0].ChoixId);
            Assert.Equal(1, etapes[0].Ordre);
        }

        [Fact]
        public async Task AppliquerChoixAsync_UpdatesScoreInDatabase()
        {
            // Summary: Vérifie qu'AppliquerChoixAsync met à jour le score dans la base de données.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var effet = new Effet { Id = Guid.NewGuid(), Type = TypeEffet.GainPoints, Valeur = 20, Description = "+20" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Gagner" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "Salle1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "Donjon1", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var joueurId = Guid.NewGuid();
            var partie = await svc.DemarrerAsync(joueurId, donjon.Id, CancellationToken.None);
            Assert.NotNull(partie);

            // Act
            await svc.AppliquerChoixAsync(partie!.Id, salle.Id, choix.Id, CancellationToken.None);

            // Assert
            var score = await db.Scores.FirstOrDefaultAsync(s => s.PartieId == partie.Id, CancellationToken.None);
            Assert.NotNull(score);
            Assert.Equal(30, score!.Valeur); // 10 initial + 20
        }
    }
}
