namespace FonTech.Domain.Interfaces.Databases;

public interface IUnitOfWork : IDisposable
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
}