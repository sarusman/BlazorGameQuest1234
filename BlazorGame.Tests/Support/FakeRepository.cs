using BlazorGame.GameService.Persistence;

namespace BlazorGame.Tests.Support
{
    /// <summary>
    /// Implémentation en mémoire d'un repository pour les tests.
    /// </summary>
    /// <typeparam name="TEntity">Type d'entité gérée (ex: Joueur, Partie, Score).</typeparam>
    public class FakeRepository<TEntity> : Repository<TEntity> where TEntity : class
    {
        private readonly List<TEntity> _store = new();

        /// <summary>
        /// Construit le fake repository sans base de données réelle.
        /// </summary>
        public FakeRepository() : base(db: null!)
        {
        }

        /// <summary>
        /// Ajoute l'entité en mémoire.
        /// </summary>
        /// <param name="entity">Entité à stocker.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>L'entité.</returns>
        public override Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _store.Add(entity);
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Retourne toutes les entités en mémoire.
        /// </summary>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Liste d'entités.</returns>
        public override Task<List<TEntity>> ListAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_store.ToList());
        }

        /// <summary>
        /// Cherche par Id si l'entité possède une propriété publique "Id" de type Guid.
        /// </summary>
        /// <param name="id">Identifiant recherché.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Entité trouvée ou null.</returns>
        public override Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var match = _store.FirstOrDefault(e =>
            {
                var prop = e!.GetType().GetProperty("Id");
                if (prop == null) { return false; }
                var value = prop.GetValue(e);
                return value is Guid g && g == id;
            });

            return Task.FromResult(match);
        }
    }
}
