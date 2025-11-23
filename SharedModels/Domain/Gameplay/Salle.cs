using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Common;
using SharedModels.Domain.Items;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Salle élémentaire d’un donjon.</summary>
    public class Salle : BaseEntity
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TypeSalle Type { get; set; } = TypeSalle.Combat;
        public Difficulte Difficulte { get; set; } = Difficulte.Normal;
        public ICollection<Choix> ChoixProposes { get; set; } = new List<Choix>();
        public ICollection<Objet>? ButinPotentiel { get; set; }
    }
}
