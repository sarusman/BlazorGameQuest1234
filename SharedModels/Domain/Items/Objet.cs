using SharedModels.Domain.Common;

namespace SharedModels.Domain.Items
{
    /// <summary>Objet utilisable ou collectable en jeu.</summary>
    public class Objet : BaseEntity
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Rareté Rarete { get; set; } = Rareté.Commun;
        public string? Code { get; set; }
    }
}