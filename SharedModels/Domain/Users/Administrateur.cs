using SharedModels.Domain.Common;

namespace SharedModels.Domain.Users
{
    /// <summary>Utilisateur de rôle administrateur lié à Keycloak.</summary>
    public class Administrateur : BaseEntity
    {
        public string Nom { get; set; } = string.Empty;
        public string KeycloakUserName { get; set; } = string.Empty;
        public bool Actif { get; set; } = true;
    }
}