using FonTech.Domain.Interfaces.Databases;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.DataAccess.Databases;

public class EfTransaction(
    IDbContextTransaction transaction
    ) : ITransaction
{
    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        await transaction.RollbackAsync(ct);
    }
}