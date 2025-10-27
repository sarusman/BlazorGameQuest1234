using SharedModels.Domain.Common;
using SharedModels.Domain.Users;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Session de jeu d’un joueur dans un donjon.</summary>
    public class Partie : BaseEntity
    {
        public Guid JoueurId { get; set; }
        public Joueur? Joueur { get; set; }
        public Guid DonjonId { get; set; }
        public Donjon? Donjon { get; set; }
        public DateTime DemarreeLe { get; set; } = DateTime.UtcNow;
        public DateTime? TermineeLe { get; set; }
        public int ScoreFinal { get; set; }
        public bool EstTerminee { get; set; }
        public ICollection<EtapeDePartie> Etapes { get; set; } = new List<EtapeDePartie>();
    }
}
