using SharedModels.Domain.Common;
using SharedModels.Domain.Common.Enums;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Conséquence d’une salle ou d’un choix.</summary>
    public class Effet : BaseEntity
    {
        public TypeEffet Type { get; set; } = TypeEffet.Rien;
        public int Valeur { get; set; }
        public string? Donnee { get; set; }
        public string? Description { get; set; }
    }
}
