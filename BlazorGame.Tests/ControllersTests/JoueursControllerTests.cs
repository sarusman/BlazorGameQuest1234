using BlazorGame.GameService.Controllers;
using BlazorGame.Tests.Support;
using SharedModels.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BlazorGame.Tests.ControllersTests
{
    /// <summary>
    /// Tests unitaires pour JoueursController (register, login, get).
    /// </summary>
    public class JoueursControllerTests
    {
        /// <summary>
        /// Vérifie que Register crée un Joueur avec le pseudo reçu.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Register_Cree_Joueur()
        {
            // arrange
            var repo = new FakeRepository<Joueur>();
            var ctrl = new JoueursController(repo);

            var req = new JoueursController.RegisterRequest
            {
                Pseudo = "PlayerOne",
                Email = "player@game.test"
            };

            // act
            var result = await ctrl.Register(req, CancellationToken.None);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueurCree = Assert.IsType<Joueur>(ok.Value);

            Assert.Equal("PlayerOne", joueurCree.Pseudo);
            Assert.NotEqual(Guid.Empty, joueurCree.Id);
        }

        /// <summary>
        /// Vérifie que Login renvoie le Joueur correspondant au pseudo donné.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Login_Retourne_Joueur_Selon_Pseudo()
        {
            // arrange
            var repo = new FakeRepository<Joueur>();

            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = "PlayerTwo"
            };

            await repo.AddAsync(joueur, CancellationToken.None);

            var ctrl = new JoueursController(repo);

            var reqLogin = new JoueursController.LoginRequest
            {
                Pseudo = "PlayerTwo"
            };

            // act
            var result = await ctrl.Login(reqLogin, CancellationToken.None);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueurRetour = Assert.IsType<Joueur>(ok.Value);

            Assert.Equal(joueur.Id, joueurRetour.Id);
            Assert.Equal("PlayerTwo", joueurRetour.Pseudo);
        }

        /// <summary>
        /// Vérifie que Login renvoie 404 si le pseudo n'existe pas.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Login_Joueur_Inexistant_Retourne_404()
        {
            // arrange
            var repo = new FakeRepository<Joueur>();
            var ctrl = new JoueursController(repo);

            var reqLogin = new JoueursController.LoginRequest
            {
                Pseudo = "Nobody"
            };

            // act
            var result = await ctrl.Login(reqLogin, CancellationToken.None);

            // assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        /// <summary>
        /// Vérifie que GetById renvoie 404 si le joueur n'existe pas.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task GetById_Inexistant_Retourne_404()
        {
            // arrange
            var repo = new FakeRepository<Joueur>();
            var ctrl = new JoueursController(repo);

            // act
            var result = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);

            // assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Vérifie que GetById retourne le bon Joueur quand l'Id existe.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task GetById_Retourne_Le_Bon_Joueur()
        {
            // arrange
            var repo = new FakeRepository<Joueur>();
            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = "Existing"
            };
            await repo.AddAsync(joueur, CancellationToken.None);

            var ctrl = new JoueursController(repo);

            // act
            var result = await ctrl.GetById(joueur.Id, CancellationToken.None);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueurRetour = Assert.IsType<Joueur>(ok.Value);

            Assert.Equal(joueur.Id, joueurRetour.Id);
            Assert.Equal("Existing", joueurRetour.Pseudo);
        }
    }
}



