using SharedModels.Domain.Common;
using SharedModels.Domain.Items;
using SharedModels.Domain.Scores;
using SharedModels.Domain.Journal;
using SharedModels.Domain.Gameplay;

namespace SharedModels.Domain.Users
{
    /// <summary>Utilisateur de rôle joueur lié à Keycloak.</summary>
    public class Joueur : BaseEntity
    {
        public string Pseudo { get; set; } = string.Empty;
        public string KeycloakUserName { get; set; } = string.Empty;
        public bool Actif { get; set; } = true;
        public ICollection<InventaireItem> Inventaire { get; set; } = new List<InventaireItem>();
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        public ICollection<Sauvegarde> Sauvegardes { get; set; } = new List<Sauvegarde>();
        public ICollection<Partie> Parties { get; set; } = new List<Partie>();
        public ICollection<HistoriqueEvenement> Evenements { get; set; } = new List<HistoriqueEvenement>();
    }
}