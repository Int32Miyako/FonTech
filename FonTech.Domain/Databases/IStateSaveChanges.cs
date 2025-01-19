namespace FonTech.Domain.Databases;

public interface IStateSaveChanges 
{
    Task<int> SaveChangesAsync();
}