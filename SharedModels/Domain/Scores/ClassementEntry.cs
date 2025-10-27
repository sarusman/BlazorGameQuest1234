using SharedModels.Domain.Common;
using SharedModels.Domain.Users;

namespace SharedModels.Domain.Scores
{
    /// <summary>Entrée agrégée pour le classement.</summary>
    public class ClassementEntry : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public int MeilleurScore { get; set; }
        public int PartiesJouees { get; set; }
        public int PartiesGagnees { get; set; }
    }
}