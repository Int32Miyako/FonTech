using FonTech.Domain.Databases;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.DataAccess.Databases;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<User> users, 
    IBaseRepository<Role> roles,
    IBaseRepository<UserRole> userRoles
    ) 
    : IUnitOfWork
{
    public IBaseRepository<User> Users { get; set; } = users;
    public IBaseRepository<Role> Roles { get; set; } = roles;
    public IBaseRepository<UserRole> UserRoles { get; set; } = userRoles;
    

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
  
}