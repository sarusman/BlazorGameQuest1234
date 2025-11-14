using System;
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
    }
}
