using BlazorGame.GameService.Controllers;
using BlazorGame.Tests.Support;
using SharedModels.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BlazorGame.Tests.ControllersTests
{
    /// <summary>
    /// Tests unitaires pour JoueursController.
    /// </summary>
    public class JoueursControllerTests
    {
        /// <summary>
        /// Vérifie que Register crée un Joueur avec les bonnes infos.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Register_Cree_Joueur()
        {
            var repo = new FakeRepository<Joueur>();
            var ctrl = new JoueursController(repo);

            var req = new JoueursController.RegisterRequest
            {
                Pseudo = "PlayerOne",
                KeycloakUserName = "kc_player_one",
                Actif = true
            };

            var result = await ctrl.Register(req, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueur = Assert.IsType<Joueur>(ok.Value);

            Assert.Equal("PlayerOne", joueur.Pseudo);
            Assert.Equal("kc_player_one", joueur.KeycloakUserName);
            Assert.True(joueur.Actif);
            Assert.NotEqual(Guid.Empty, joueur.Id);
        }

        /// <summary>
        /// Vérifie que Login renvoie le joueur actif correspondant au KeycloakUserName.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Login_Retourne_Joueur_Actif()
        {
            var repo = new FakeRepository<Joueur>();

            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = "PlayerTwo",
                KeycloakUserName = "kc_two",
                Actif = true
            };

            await repo.AddAsync(joueur, CancellationToken.None);

            var ctrl = new JoueursController(repo);

            var reqLogin = new JoueursController.LoginRequest
            {
                KeycloakUserName = "kc_two"
            };

            var result = await ctrl.Login(reqLogin, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueurRetour = Assert.IsType<Joueur>(ok.Value);

            Assert.Equal(joueur.Id, joueurRetour.Id);
            Assert.Equal("kc_two", joueurRetour.KeycloakUserName);
        }

        /// <summary>
        /// Vérifie que GetById renvoie 404 si le joueur n'existe pas.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task GetById_Inexistant_Retourne_404()
        {
            var repo = new FakeRepository<Joueur>();
            var ctrl = new JoueursController(repo);

            var result = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
