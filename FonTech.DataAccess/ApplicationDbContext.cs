using System.Reflection;
using FonTech.DataAccess.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FonTech.DataAccess;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // надо зарегать как синглтон
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /* идеальный способ подключения необходимых данных
         из сборки проекта сборка.исполняемаяСборка */
        
        // идеальный код? Прям имба инфа?
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}