using FonTech.DataAccess.Interceptors;
using FonTech.DataAccess.Repositories;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FonTech.DataAccess.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");
        services.AddDbContext<ApplicationDbContext>(options 
            => options.UseNpgsql(connectionString));
        
        
        services.AddSingleton<DateInterceptor>();
        // если мы расширяем this IServiceCollection services то можно таким образом красиво ёбнуть
        services.InitRepositories();
    }

    public static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
    }
}