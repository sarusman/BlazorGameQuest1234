namespace SharedModels.Domain.Gameplay;
public class StartPartieRequest
{
    public Guid JoueurId { get; set; }
    public Guid DonjonId { get; set; }
}