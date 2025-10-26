using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.Tests.Support;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BlazorGame.Tests.ControllersTests
{
    /// <summary>
    /// Tests unitaires pour PartiesController.
    /// </summary>
    public class PartiesControllerTests
    {
        /// <summary>
        /// Vérifie que Start crée une partie pour un joueur valide.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Start_Cree_Partie_Pour_Joueur()
        {
            var joueurRepo = new FakeRepository<Joueur>();
            var partieRepo = new FakeRepository<Partie>();
            var gameplay = new GameplayService();

            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = "Runner",
                KeycloakUserName = "kc_runner",
                Actif = true
            };
            await joueurRepo.AddAsync(joueur, CancellationToken.None);

            var ctrl = new PartiesController(joueurRepo, partieRepo, gameplay);

            var req = new PartiesController.StartRequest
            {
                JoueurId = joueur.Id
            };

            var result = await ctrl.Start(req, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var partieCree = Assert.IsType<Partie>(ok.Value);

            Assert.Equal(joueur.Id, partieCree.JoueurId);
            Assert.False(partieCree.EstTerminee);
            Assert.NotEqual(Guid.Empty, partieCree.Id);
        }

        /// <summary>
        /// Vérifie que GetById retourne 404 si la partie n'existe pas.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task GetById_Inexistant_Retourne_404()
        {
            var joueurRepo = new FakeRepository<Joueur>();
            var partieRepo = new FakeRepository<Partie>();
            var gameplay = new GameplayService();

            var ctrl = new PartiesController(joueurRepo, partieRepo, gameplay);

            var result = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
