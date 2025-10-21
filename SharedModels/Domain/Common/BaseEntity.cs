namespace SharedModels.Domain.Common
{
    /// <summary>Entité de base avec identifiant.</summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
