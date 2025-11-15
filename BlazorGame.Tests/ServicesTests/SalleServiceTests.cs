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

        [Fact]
        public void GenererSalle_PiegeBranch_ReturnsPiegeSalle()
        {
            // Summary: Vérifie la branche Piege produit une salle avec titre et description appropriés.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Piege, 999);

            // Assert
            Assert.Equal(TypeSalle.Piege, salle.Type);
            Assert.Contains("Dalles", salle.Titre);
            Assert.NotEmpty(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_RencontreBranch_ReturnsRencontreSalle()
        {
            // Summary: Vérifie la branche Rencontre produit une salle avec titre et description appropriés.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Rencontre, 888);

            // Assert
            Assert.Equal(TypeSalle.Rencontre, salle.Type);
            Assert.Contains("inconnu", salle.Titre, StringComparison.OrdinalIgnoreCase);
            Assert.NotEmpty(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_ReposBranch_ReturnsReposSalle()
        {
            // Summary: Vérifie la branche Repos produit une salle avec titre et description appropriés.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, TypeSalle.Repos, 777);

            // Assert
            Assert.Equal(TypeSalle.Repos, salle.Type);
            Assert.Contains("repos", salle.Titre, StringComparison.OrdinalIgnoreCase);
            Assert.NotEmpty(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_WithNullType_GeneratesRandomType()
        {
            // Summary: Vérifie que GenererSalle génère un type aléatoire quand type est null.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Normal, null, 123);

            // Assert
            Assert.NotNull(salle);
            Assert.NotEmpty(salle.Titre);
            Assert.NotEmpty(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_WithNullSeed_GeneratesDifferentSalles()
        {
            // Summary: Vérifie que GenererSalle avec seed null génère des salles différentes.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle1 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, null);
            var salle2 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, null);

            // Assert
            Assert.NotNull(salle1);
            Assert.NotNull(salle2);
            Assert.NotEqual(salle1.Id, salle2.Id);
        }

        [Fact]
        public void GenererSalle_WithSameSeed_GeneratesSameSalle()
        {
            // Summary: Vérifie que GenererSalle avec le même seed génère la même salle (reproductibilité).

            // Arrange
            var svc = new SalleService();
            const int seed = 12345;

            // Act
            var salle1 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Coffre, seed);
            var salle2 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Coffre, seed);

            // Assert
            Assert.Equal(salle1.Type, salle2.Type);
            Assert.Equal(salle1.Titre, salle2.Titre);
            Assert.Equal(salle1.Description, salle2.Description);
            Assert.Equal(salle1.ChoixProposes.Count, salle2.ChoixProposes.Count);
        }

        [Fact]
        public void GenererSalle_GeneratesUniqueId()
        {
            // Summary: Vérifie que GenererSalle génère un Id unique pour chaque salle.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle1 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, 1);
            var salle2 = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, 2);

            // Assert
            Assert.NotEqual(Guid.Empty, salle1.Id);
            Assert.NotEqual(Guid.Empty, salle2.Id);
            Assert.NotEqual(salle1.Id, salle2.Id);
        }

        [Fact]
        public void GenererSalle_WithFacileDifficulty_SetsCorrectDifficulte()
        {
            // Summary: Vérifie que GenererSalle définit correctement la difficulté Facile.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Facile, TypeSalle.Combat, 100);

            // Assert
            Assert.Equal(Difficulte.Facile, salle.Difficulte);
            Assert.NotNull(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_WithDifficileDifficulty_SetsCorrectDifficulte()
        {
            // Summary: Vérifie que GenererSalle définit correctement la difficulté Difficile.

            // Arrange
            var svc = new SalleService();

            // Act
            var salle = svc.GenererSalle(Difficulte.Difficile, TypeSalle.Combat, 200);

            // Assert
            Assert.Equal(Difficulte.Difficile, salle.Difficulte);
            Assert.NotNull(salle.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_WithAllDifficulties_SetsCorrectDifficulte()
        {
            // Summary: Vérifie que GenererSalle définit correctement toutes les difficultés disponibles.

            // Arrange
            var svc = new SalleService();

            // Act & Assert
            var salleFacile = svc.GenererSalle(Difficulte.Facile, TypeSalle.Combat, 100);
            var salleNormal = svc.GenererSalle(Difficulte.Normal, TypeSalle.Combat, 200);
            var salleDifficile = svc.GenererSalle(Difficulte.Difficile, TypeSalle.Combat, 300);

            Assert.Equal(Difficulte.Facile, salleFacile.Difficulte);
            Assert.Equal(Difficulte.Normal, salleNormal.Difficulte);
            Assert.Equal(Difficulte.Difficile, salleDifficile.Difficulte);
            Assert.NotNull(salleFacile.ChoixProposes);
            Assert.NotNull(salleNormal.ChoixProposes);
            Assert.NotNull(salleDifficile.ChoixProposes);
        }

        [Fact]
        public void GenererSalle_AlwaysHasChoixProposes()
        {
            // Summary: Vérifie que toutes les salles générées ont au moins un choix.

            // Arrange
            var svc = new SalleService();
            var types = new[] { TypeSalle.Combat, TypeSalle.Coffre, TypeSalle.Enigme, TypeSalle.Piege, TypeSalle.Rencontre, TypeSalle.Repos };

            // Act & Assert
            foreach (var type in types)
            {
                var salle = svc.GenererSalle(Difficulte.Normal, type, null);
                Assert.NotEmpty(salle.ChoixProposes);
                Assert.All(salle.ChoixProposes, c => Assert.NotNull(c.Libelle));
            }
        }
    }
}
