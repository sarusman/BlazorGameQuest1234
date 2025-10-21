using SharedModels.Domain.Common;

namespace SharedModels.Domain.Gameplay
{
    /// <summary>Donjon généré aléatoirement pour une partie.</summary>
    public class Donjon : BaseEntity
    {
        public string Nom { get; set; } = string.Empty;
        public Difficulte Difficulte { get; set; } = Difficulte.Normal;
        public int NbMaxSalles { get; set; } = 5;
        public ICollection<Salle> Salles { get; set; } = new List<Salle>();
    }
}