using FonTech.Domain.Interfaces.Repositories;

namespace FonTech.DataAccess.Repositories;


public class BaseRepository<TEntity>(ApplicationDbContext dbContext) : IBaseRepository<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetAll()
    {
        // установка какие объекты мы будем вытаскивать
        // обязательно пишем where TEntity : class
        return dbContext.Set<TEntity>();
    }

    
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.AddAsync(entity, ct); 

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Update(entity);

        return entity;
    }


    public async Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Remove(entity);

        return entity;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }
    
}