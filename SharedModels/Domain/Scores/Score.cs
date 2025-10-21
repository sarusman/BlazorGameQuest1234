using SharedModels.Domain.Common;
using SharedModels.Domain.Users;

namespace SharedModels.Domain.Scores
{
    /// <summary>Score final enregistré pour une partie.</summary>
    public class Score : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public Guid PartieId { get; set; }
        public int Valeur { get; set; }
        public DateTime EnregistreLe { get; set; } = DateTime.UtcNow;
    }
}