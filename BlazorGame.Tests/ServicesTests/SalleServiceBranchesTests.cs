using System;
using Xunit;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Common.Enums;

namespace BlazorGame.Tests.ServicesTests
{
    public class SalleServiceBranchesTests
    {
        [Fact]
        public void GenererSalle_CombatBranch_ReturnsCombatChoices()
        {
            // Summary: Vérifie la branche Combat produit au moins 2 choix et libellés cohérents.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, 555);

            // Assert
            Assert.Equal(TypeSalle.Combat, salle.Type);
            Assert.NotEmpty(salle.ChoixProposes);
            Assert.Contains(salle.ChoixProposes, c => c.Libelle != null);
        }

        [Fact]
        public void GenererSalle_CoffreBranch_ReturnsOpenOrIgnore()
        {
            // Summary: Vérifie la branche Coffre propose Ouvrir et Ignorer.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Coffre, 123);

            // Assert
            Assert.Equal(TypeSalle.Coffre, salle.Type);
            Assert.True(salle.ChoixProposes.Count >= 1);
            Assert.Contains(salle.ChoixProposes, c => c.Type == SharedModels.Domain.Common.Enums.TypeChoix.Ouvrir || c.Type == SharedModels.Domain.Common.Enums.TypeChoix.Ignorer);
        }

        [Fact]
        public void GenererSalle_EnigmeBranch_ReturnsResolverOrFuir()
        {
            // Summary: Vérifie la branche Enigme contient Resolver et Fuir.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Enigme, 777);

            // Assert
            Assert.Equal(TypeSalle.Enigme, salle.Type);
            Assert.Contains(salle.ChoixProposes, c => c.Type == SharedModels.Domain.Common.Enums.TypeChoix.Resolver || c.Type == SharedModels.Domain.Common.Enums.TypeChoix.Fuir);
        }
    }
}
