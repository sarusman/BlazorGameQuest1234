using SharedModels.Domain.Common;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;

namespace SharedModels.Domain.Journal
{
    /// <summary>Instantané pour reprendre une partie.</summary>
    public class Sauvegarde : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public Guid PartieId { get; set; }
        public Guid EtapeCouranteId { get; set; }
        public string EtatJson { get; set; } = string.Empty;
        public DateTime EnregistreeLe { get; set; } = DateTime.UtcNow;
    }
}