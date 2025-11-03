namespace BlazorGame.Client.Services
{
    /// <summary>État de partie côté client (score, journal, verrous).</summary>
    public class GameState
    {
        /// <summary>Score courant.</summary>
        public int Score { get; private set; }

        /// <summary>Id du donjon en cours.</summary>
        public Guid? DonjonId { get; private set; }

        /// <summary>Id de la partie en cours (si gérée côté serveur).</summary>
        public Guid? PartieId { get; private set; }

        /// <summary>Journal des actions.</summary>
        public List<string> Journal { get; } = new();

        /// <summary>Partie terminée ?</summary>
        public bool Finished { get; private set; }

        /// <summary>Réinitialise score et journal (appelé à la première salle).</summary>
        /// <returns>Rien.</returns>
        public void Reset()
        {
            Score = 0;
            Finished = false;
            Journal.Clear();
        }

        /// <summary>Réinitialise pour un donjon/partie précis.</summary>
        /// <param name="donjonId">Id du donjon.</param>
        /// <param name="partieId">Id de la partie.</param>
        /// <returns>Rien.</returns>
        public void ResetForDonjon(Guid donjonId, Guid partieId)
        {
            DonjonId = donjonId;
            PartieId = partieId;
            Reset();
        }

        /// <summary>Applique un delta de score et ajoute une note au journal.</summary>
        /// <param name="delta">Variation de score (±).</param>
        /// <param name="note">Texte de journal.</param>
        /// <returns>Rien.</returns>
        public void Apply(int delta, string note)
        {
            Score += delta;
            if (!string.IsNullOrWhiteSpace(note)) Journal.Add(note);
        }

        /// <summary>Marque la partie comme terminée.</summary>
        /// <returns>Rien.</returns>
        public void SetFinished() => Finished = true;
    }
}
