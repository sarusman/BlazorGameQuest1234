namespace SharedModels.Domain.Users;

/// <summary>
/// Modèle pour la connexion.
/// </summary>
public class LoginRequest
{
    /// <summary>Pseudo du joueur.</summary>
    public string Pseudo { get; set; } = string.Empty;
}
