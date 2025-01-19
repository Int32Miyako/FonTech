using FonTech.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace FonTech.Consumer.DependencyInjection;

public static class DependencyInjection
{
    public static void AddConsumer(this IServiceCollection services)
    {
        // если используется хостед сервис и это бэкграунд джоба то не используем скоуп
        services.AddHostedService<RabbitMqListener>();
    }
}