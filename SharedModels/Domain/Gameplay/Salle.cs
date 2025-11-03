using SharedModels.Domain.Common;
using SharedModels.Domain.Items;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Salle élémentaire d’un donjon.</summary>
    public class Salle : BaseEntity
    {
        /// <summary>Titre de la salle.</summary>
        public string Titre { get; set; } = string.Empty;
        /// <summary>Description de la salle.</summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>Type de salle.</summary>
        public TypeSalle Type { get; set; } = TypeSalle.Combat;
        /// <summary>Difficulté de la salle.</summary>
        public Difficulte Difficulte { get; set; } = Difficulte.Normal;
        /// <summary>Choix proposés au joueur.</summary>
        public ICollection<Choix> ChoixProposes { get; set; } = new List<Choix>();
        /// <summary>Butin potentiel (facultatif).</summary>
        public ICollection<Objet>? ButinPotentiel { get; set; }
    }
}
