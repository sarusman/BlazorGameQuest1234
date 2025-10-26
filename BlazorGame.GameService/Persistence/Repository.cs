using Microsoft.EntityFrameworkCore;

namespace BlazorGame.GameService.Persistence
{
    /// <summary>
    /// Accès générique lecture/écriture pour une entité EF Core.
    /// </summary>
    /// <typeparam name="TEntity">Type d'entité persistée.</typeparam>
    public class Repository<TEntity> where TEntity : class
    {
        private readonly GameDbContext _db;

        /// <summary>
        /// Construit le repository.
        /// </summary>
        /// <param name="db">Contexte EF Core injecté.</param>
        public Repository(GameDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Ajoute une entité et sauvegarde en base.
        /// </summary>
        /// <param name="entity">Entité à créer.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>L'entité insérée.</returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _db.Set<TEntity>().Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        /// <summary>
        /// Récupère une entité par Id (GUID).
        /// </summary>
        /// <param name="id">Identifiant recherché.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>L'entité ou null.</returns>
        public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Set<TEntity>().FindAsync([id], ct);
        }

        /// <summary>
        /// Retourne toutes les entités du type demandé.
        /// </summary>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Liste d'entités.</returns>
        public virtual async Task<List<TEntity>> ListAsync(CancellationToken ct = default)
        {
            return await _db.Set<TEntity>().ToListAsync(ct);
        }
    }
}
