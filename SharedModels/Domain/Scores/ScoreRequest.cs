namespace SharedModels.Domain.Scores;

/// <summary>Payload création score.</summary>
public class ScoreRequest
{
    public Guid JoueurId { get; set; }
    public Guid PartieId { get; set; }
    public int Valeur { get; set; }
}
