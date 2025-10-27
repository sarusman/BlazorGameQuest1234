using SharedModels.Domain.Common;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;

namespace SharedModels.Domain.Journal
{
    /// <summary>Trace textuelle d’un événement de jeu.</summary>
    public class HistoriqueEvenement : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public Guid? PartieId { get; set; }
        public Guid? EtapeId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
    }
}