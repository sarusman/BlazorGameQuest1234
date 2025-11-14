using System;
using Xunit;
using SharedModels.Domain.Common.Enums;
using BlazorGame.GameService.Services;

namespace BlazorGame.Tests.ServicesTests
{
    public class SalleServiceTests
    {
        [Fact]
        public void GenererSalle_WithTypeAndSeed_ReturnsExpectedTypeAndChoix()
        {
            // Summary: Vérifie que la génération d'une salle avec type/seed renvoie une salle cohérente.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, SharedModels.Domain.Common.Enums.TypeSalle.Coffre, 123);

            // Assert
            Assert.NotNull(salle);
            Assert.Equal(SharedModels.Domain.Common.Enums.TypeSalle.Coffre, salle.Type);
            Assert.False(string.IsNullOrWhiteSpace(salle.Titre));
            Assert.NotEmpty(salle.ChoixProposes);
        }
    }
}
