using SharedModels.Domain.Common;
using SharedModels.Domain.Users;

namespace SharedModels.Domain.Items
{
    /// <summary>Lien entre un joueur et un objet possédé.</summary>
    public class InventaireItem : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public Guid ObjetId { get; set; }
        public Objet? Objet { get; set; }
        public int Quantite { get; set; } = 1;
    }
}