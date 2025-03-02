namespace FonTech.Domain.Interfaces.Databases;

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}