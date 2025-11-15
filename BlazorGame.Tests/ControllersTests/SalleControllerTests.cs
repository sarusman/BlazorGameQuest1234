using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ControllersTests
{
    public class SalleControllerTests
    {
        [Fact]
        public void Random_ReturnsOkWithSalle()
        {
            // Summary: Vérifie que l'endpoint Random renvoie une salle via OkObjectResult.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act
            var result = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, 42);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public void Random_WithDifferentDifficulties_ReturnsOk()
        {
            // Summary: Vérifie que Random fonctionne avec différents niveaux de difficulté.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act & Assert
            var facile = ctrl.Random(Difficulte.Facile, null, null);
            var normal = ctrl.Random(Difficulte.Normal, null, null);
            var difficile = ctrl.Random(Difficulte.Difficile, null, null);

            Assert.IsType<OkObjectResult>(facile.Result);
            Assert.IsType<OkObjectResult>(normal.Result);
            Assert.IsType<OkObjectResult>(difficile.Result);
        }

        [Fact]
        public void Random_WithTypeSpecified_ReturnsCorrectType()
        {
            // Summary: Vérifie que Random retourne une salle du type spécifié.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act
            var result = ctrl.Random(Difficulte.Normal, TypeSalle.Coffre, 123);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var salle = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok.Value);
            Assert.Equal(TypeSalle.Coffre, salle.Type);
        }

        [Fact]
        public void Random_WithNullType_ReturnsRandomType()
        {
            // Summary: Vérifie que Random retourne une salle avec un type aléatoire quand type est null.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act
            var result = ctrl.Random(Difficulte.Normal, null, 456);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var salle = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok.Value);
            Assert.NotEmpty(salle.Titre);
        }

        [Fact]
        public void Random_WithSeed_ReturnsReproducibleResult()
        {
            // Summary: Vérifie que Random avec seed retourne un résultat reproductible.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);
            const int seed = 789;

            // Act
            var result1 = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, seed);
            var result2 = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, seed);

            // Assert
            var ok1 = Assert.IsType<OkObjectResult>(result1.Result);
            var ok2 = Assert.IsType<OkObjectResult>(result2.Result);
            var salle1 = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok1.Value);
            var salle2 = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok2.Value);
            Assert.Equal(salle1.Titre, salle2.Titre);
            Assert.Equal(salle1.Description, salle2.Description);
        }

        [Fact]
        public void Random_WithNullSeed_ReturnsDifferentResults()
        {
            // Summary: Vérifie que Random avec seed null retourne des résultats différents.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act
            var result1 = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, null);
            var result2 = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, null);

            // Assert
            var ok1 = Assert.IsType<OkObjectResult>(result1.Result);
            var ok2 = Assert.IsType<OkObjectResult>(result2.Result);
            var salle1 = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok1.Value);
            var salle2 = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok2.Value);
            Assert.NotEqual(salle1.Id, salle2.Id);
        }

        [Fact]
        public void Random_WithDefaultParameters_ReturnsOk()
        {
            // Summary: Vérifie que Random fonctionne avec les paramètres par défaut (difficulté Normal).

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act - Appel sans paramètres (utilise les valeurs par défaut)
            var result = ctrl.Random();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var salle = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok.Value);
            Assert.NotNull(salle);
            Assert.NotEmpty(salle.Titre);
        }

        [Fact]
        public void Random_ReturnsSalleWithChoixProposes()
        {
            // Summary: Vérifie que Random retourne une salle avec des choix proposés.

            // Arrange
            var svc = new SalleService();
            var ctrl = new SalleController(svc);

            // Act
            var result = ctrl.Random(Difficulte.Normal, TypeSalle.Combat, 999);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var salle = Assert.IsType<SharedModels.Domain.Gameplay.Salle>(ok.Value);
            Assert.NotEmpty(salle.ChoixProposes);
            Assert.All(salle.ChoixProposes, c => Assert.NotNull(c.Libelle));
        }
    }
}
