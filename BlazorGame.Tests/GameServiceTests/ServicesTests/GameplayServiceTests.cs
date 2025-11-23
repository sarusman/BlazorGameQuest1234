using System;
using System.Linq;
using Xunit;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Users;

namespace BlazorGame.Tests.ServicesTests
{
    public class GameplayServiceTests
    {
        [Fact]
        public void CreerNouvellePartie_ReturnsPartieWithDonjonAndSalle()
        {
            // Summary: Vérifie qu'une nouvelle partie contient un donjon et une salle de départ.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie = svc.CreerNouvellePartie(player);

            // Assert
            Assert.NotNull(partie);
            Assert.NotNull(partie.Donjon);
            Assert.NotNull(partie.Donjon.Salles);
            Assert.Single(partie.Donjon.Salles);
        }

        [Fact]
        public void CreerNouvellePartie_CreatesPartieWithCorrectProperties()
        {
            // Summary: Vérifie que CreerNouvellePartie crée une partie avec les propriétés correctes.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie = svc.CreerNouvellePartie(player);

            // Assert
            Assert.NotNull(partie);
            Assert.NotEqual(Guid.Empty, partie.Id);
            Assert.Equal(player.Id, partie.JoueurId);
            Assert.NotNull(partie.Joueur);
            Assert.Equal(player.Id, partie.Joueur!.Id);
        }

        [Fact]
        public void CreerNouvellePartie_CreatesDonjonWithCorrectProperties()
        {
            // Summary: Vérifie que CreerNouvellePartie crée un donjon avec les propriétés correctes.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie = svc.CreerNouvellePartie(player);

            // Assert
            Assert.NotNull(partie.Donjon);
            Assert.NotEqual(Guid.Empty, partie.Donjon!.Id);
            Assert.Equal("Donjon du Dragon", partie.Donjon.Nom);
            Assert.Equal(partie.DonjonId, partie.Donjon.Id);
        }

        [Fact]
        public void CreerNouvellePartie_CreatesSalleDepartWithCorrectProperties()
        {
            // Summary: Vérifie que CreerNouvellePartie crée une salle de départ avec les propriétés correctes.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie = svc.CreerNouvellePartie(player);

            // Assert
            Assert.NotNull(partie.Donjon);
            Assert.Single(partie.Donjon!.Salles);
            var salle = partie.Donjon.Salles.First();
            Assert.NotEqual(Guid.Empty, salle.Id);
            Assert.Equal("Entrée du donjon", salle.Titre);
            Assert.Contains("couloir", salle.Description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreerNouvellePartie_EstablishesCorrectRelations()
        {
            // Summary: Vérifie que CreerNouvellePartie établit correctement les relations entre entités.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie = svc.CreerNouvellePartie(player);

            // Assert
            Assert.Equal(partie.JoueurId, partie.Joueur!.Id);
            Assert.Equal(partie.DonjonId, partie.Donjon!.Id);
            Assert.NotNull(partie.Donjon.Salles);
            Assert.Single(partie.Donjon.Salles);
        }

        [Fact]
        public void CreerNouvellePartie_GeneratesUniqueIds()
        {
            // Summary: Vérifie que CreerNouvellePartie génère des Ids uniques pour chaque partie.

            // Arrange
            var player = new Joueur { Id = Guid.NewGuid(), Pseudo = "test" };
            var svc = new GameplayService();

            // Act
            var partie1 = svc.CreerNouvellePartie(player);
            var partie2 = svc.CreerNouvellePartie(player);

            // Assert
            Assert.NotEqual(partie1.Id, partie2.Id);
            Assert.NotEqual(partie1.Donjon!.Id, partie2.Donjon!.Id);
            Assert.NotEqual(partie1.Donjon.Salles.First().Id, partie2.Donjon.Salles.First().Id);
        }
    }
}
