namespace SharedModels.Domain.Gameplay;

public class ChoisirResponse
{
    public int Score { get; set; }
    public bool Mort { get; set; }
    public bool Fini { get; set; }
    public Guid? NextSalleId { get; set; }
}