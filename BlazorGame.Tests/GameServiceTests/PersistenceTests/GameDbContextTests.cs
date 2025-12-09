using System;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Scores;

namespace BlazorGame.Tests.GameServiceTests.PersistenceTests
{
    public class GameDbContextTests
    {
        [Fact]
        public void OnModelCreating_ConfiguresPseudoIndex()
        {
            // Summary: Vérifie que l'index unique sur Pseudo est créé

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);
            var model = db.Model;
            var joueurEntity = model.FindEntityType(typeof(Joueur));

            // Assert
            Assert.NotNull(joueurEntity);
            var pseudoIndex = joueurEntity!.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Pseudo"));
            Assert.NotNull(pseudoIndex);
            Assert.True(pseudoIndex!.IsUnique);
        }

        [Fact]
        public void OnModelCreating_ConfiguresPartieJoueurRelation()
        {
            // Summary: Vérifie que la relation Partie->Joueur est configurée avec Cascade

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);
            var model = db.Model;
            var partieEntity = model.FindEntityType(typeof(Partie));

            // Assert
            Assert.NotNull(partieEntity);
            var navigation = partieEntity!.GetNavigations().FirstOrDefault(n => n.Name == "Joueur");
            Assert.NotNull(navigation);
        }

        [Fact]
        public void OnModelCreating_ConfiguresScoreJoueurRelation()
        {
            // Summary: Vérifie que la relation Score->Joueur est configurée

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);
            var model = db.Model;
            var scoreEntity = model.FindEntityType(typeof(Score));

            // Assert
            Assert.NotNull(scoreEntity);
            var navigation = scoreEntity!.GetNavigations().FirstOrDefault(n => n.Name == "Joueur");
            Assert.NotNull(navigation);
        }

        [Fact]
        public void OnModelCreating_IgnoresButinPotentiel()
        {
            // Summary: Vérifie que la propriété ButinPotentiel de Salle est ignorée

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);
            var model = db.Model;
            var salleEntity = model.FindEntityType(typeof(Salle));

            // Assert
            Assert.NotNull(salleEntity);
            var butin = salleEntity!.FindProperty("ButinPotentiel");
            Assert.Null(butin); // La propriété doit être ignorée
        }

        [Fact]
        public void OnModelCreating_ConfiguresDonjonSallesCascade()
        {
            // Summary: Vérifie que la relation Donjon->Salles est configurée avec Cascade

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);
            var model = db.Model;
            var donjonEntity = model.FindEntityType(typeof(Donjon));

            // Assert
            Assert.NotNull(donjonEntity);
            var navigation = donjonEntity!.GetNavigations().FirstOrDefault(n => n.Name == "Salles");
            Assert.NotNull(navigation);
        }

        [Fact]
        public void DbSets_AreAccessible()
        {
            // Summary: Vérifie que tous les DbSets sont accessibles

            // Arrange & Act
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new GameDbContext(opts);

            // Assert
            Assert.NotNull(db.Joueurs);
            Assert.NotNull(db.Parties);
            Assert.NotNull(db.Etapes);
            Assert.NotNull(db.Scores);
            Assert.NotNull(db.Donjons);
            Assert.NotNull(db.Salles);
            Assert.NotNull(db.Choix);
            Assert.NotNull(db.Effets);
        }
    }
}
