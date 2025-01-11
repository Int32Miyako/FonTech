using FonTech.Domain.Interfaces.Repositories;

namespace FonTech.DataAccess.Repositories;


public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _dbContext;
    
    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IQueryable<TEntity> GetAll()
    {
        // установка какие объекты мы будем вытаскивать
        // обязательно пишем where TEntity : class
        return _dbContext.Set<TEntity>();
    }

    
    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbContext.AddAsync(entity, ct); // Асинхронное добавление
         await _dbContext.SaveChangesAsync(ct); // Асинхронное сохранение

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbContext.Update(entity);
         await _dbContext.SaveChangesAsync(ct);

        return entity;
    }


    public async Task<TEntity> RemoveAsync(TEntity entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);

        return entity;
    }

}