namespace SharedModels.Domain.Users;
public class RegisterRequest
{
    /// <summary>Le pseudo choisi par le joueur.</summary>
    public string Pseudo { get; set; } = string.Empty;

    /// <summary>Email du joueur.</summary>
    public string Email { get; set; } = string.Empty;
}
