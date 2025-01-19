using FonTech.Domain.Databases;

namespace FonTech.Domain.Interfaces.Repositories;

// общий репозиторий
public interface IBaseRepository<TEntity> : IStateSaveChanges
{
    IQueryable<TEntity> GetAll();

    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct);

    Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct);
}