using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using SharedModels.Domain.Common.Enums;

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
    }
}
