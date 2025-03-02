using FonTech.Domain.Interfaces.Databases;

namespace FonTech.Domain.Interfaces.Repositories;

// общий репозиторий
public interface IBaseRepository<TEntity>
{
    IQueryable<TEntity> GetAll();

    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct);

    Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct);
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}