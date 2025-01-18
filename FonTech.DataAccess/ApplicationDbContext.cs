using System.Reflection;
using FonTech.DataAccess.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FonTech.DataAccess;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        /*
         *   Чтобы создать новую миграцию, просто используйте команду:
         *  dotnet ef migrations add <MigrationName> -p FonTech.DataAccess -s FonTech.Api
         *  Пример:
         *  dotnet ef migrations add AddUserTable -p FonTech.DataAccess -s FonTech.Api
         *
         *  Чтобы применить определенную миграцию, используйте команду с именем миграции:
         *  dotnet ef database update <MigrationName> -p FonTech.DataAccess -s FonTech.Api
         * 
         */
        
        try
        {
            Database.EnsureCreated(); // дурею с этой прикормки
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
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