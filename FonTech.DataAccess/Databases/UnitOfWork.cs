using FonTech.Domain.Interfaces.Databases;

namespace FonTech.DataAccess.Databases;

public class UnitOfWork(
    ApplicationDbContext context
    ) : IUnitOfWork
{
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var efTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(efTransaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}