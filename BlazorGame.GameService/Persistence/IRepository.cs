using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorGame.GameService.Persistence
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<TEntity>> ListAsync(CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    }
}