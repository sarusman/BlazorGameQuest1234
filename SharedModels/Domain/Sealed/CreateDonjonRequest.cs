using SharedModels.Domain.Common;

namespace SharedModels.Domain.Sealed;

public sealed class CreateDonjonRequest
{
    public string? Nom { get; set; }
    public Difficulte Difficulte { get; set; }
    public int NbSalles { get; set; }
    public int? Seed { get; set; }
}