using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Common;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Action décidée par le joueur dans une salle.</summary>
    public class Choix : BaseEntity
    {
        public TypeChoix Type { get; set; } = TypeChoix.Ignorer;
        public string Libelle { get; set; } = string.Empty;
        public ICollection<Effet> Effets { get; set; } = new List<Effet>();
    }
}