using SharedModels.Domain.Common;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Conséquence d’une salle ou d’un choix.</summary>
    public class Effet : BaseEntity
    {
        public TypeEffet Type { get; set; } = TypeEffet.Rien;
        public int Valeur { get; set; }
        public string? Donnee { get; set; }
    }
}
