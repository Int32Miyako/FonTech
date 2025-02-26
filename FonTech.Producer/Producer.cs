using System.Diagnostics;
using System.Text;
using FonTech.Domain.Interfaces.Producer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FonTech.Producer;

public class Producer : IMessageProducer
{
    public async void SendMessage<T>(T message, string routingKey, string? exchange = default)
    {
        try
        {
            // коннекшн с кроликом
            var factory = new ConnectionFactory { HostName = "localhost" };

            // factory.Uri = new Uri("");
        
            // канал для взаимодействия с кроликом
            var connection = await factory.CreateConnectionAsync();

            // канал передачи
            await using var channel = await connection.CreateChannelAsync();

        
            var json = JsonConvert.SerializeObject(message, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            var body = Encoding.UTF8.GetBytes(json);


            if (exchange != null)
                await channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    body: body);
            
            
          
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}