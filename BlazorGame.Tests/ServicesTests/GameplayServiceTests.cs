using BlazorGame.GameService.Services;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using Xunit;

namespace BlazorGame.Tests.ServicesTests
{
    /// <summary>
    /// Tests unitaires pour GameplayService.
    /// </summary>
    public class GameplayServiceTests
    {
        /// <summary>
        /// Vérifie que CreerNouvellePartie génère une Partie cohérente pour un joueur donné.
        /// </summary>
        [Fact]
        public void CreerNouvellePartie_GenerePartiePourJoueur()
        {
            var service = new GameplayService();

            var joueur = new Joueur
            {
                Id = Guid.NewGuid(),
                Pseudo = "Hero",
                KeycloakUserName = "kc_hero",
                Actif = true
            };

            Partie partie = service.CreerNouvellePartie(joueur);

            Assert.NotNull(partie);
            Assert.NotEqual(Guid.Empty, partie.Id);

            Assert.Equal(joueur.Id, partie.JoueurId);
            Assert.Same(joueur, partie.Joueur);

            Assert.False(partie.EstTerminee);
            Assert.True(partie.DemarreeLe <= DateTime.UtcNow);

            Assert.NotNull(partie.Donjon);
            Assert.NotEqual(Guid.Empty, partie.DonjonId);
            Assert.NotEmpty(partie.Donjon!.Salles);
        }
    }
}
