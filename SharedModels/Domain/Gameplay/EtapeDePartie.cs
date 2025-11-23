using SharedModels.Domain.Common;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Passage d’une salle pendant une partie.</summary>
    public class EtapeDePartie : BaseEntity
    {
        public Guid PartieId { get; set; }
        public Partie? Partie { get; set; }
        public Guid SalleId { get; set; }
        public Salle? Salle { get; set; }
        public int Ordre { get; set; }
        public Guid? ChoixId { get; set; }
        public Choix? Choix { get; set; }
        public int ScoreApres { get; set; }
        public bool MortIci { get; set; }
    }
}