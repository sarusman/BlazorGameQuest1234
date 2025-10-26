using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Users;

namespace BlazorGame.GameService.Services
{
    /// <summary>
    /// Service métier gameplay : création d'une nouvelle partie et génération de donjon.
    /// </summary>
    public class GameplayService
    {
        /// <summary>
        /// Crée une nouvelle partie basique avec un donjon et une salle de départ.
        /// </summary>
        /// <param name="joueur">Joueur propriétaire de la partie.</param>
        /// <returns>Partie initialisée prête à être enregistrée.</returns>
        public Partie CreerNouvellePartie(Joueur joueur)
        {
            var salleDepart = new Salle
            {
                Id = Guid.NewGuid(),
                Titre = "Entrée du donjon",
                Description = "Un couloir humide éclairé par une torche qui grésille."
            };

            var donjon = new Donjon
            {
                Id = Guid.NewGuid(),
                Nom = "Donjon du Dragon",
                Salles = new List<Salle> { salleDepart }
            };

            var partie = new Partie
            {
                Id = Guid.NewGuid(),
                JoueurId = joueur.Id,
                Joueur = joueur,
                DonjonId = donjon.Id,
                Donjon = donjon,
            };

            return partie;
        }
    }
}
