using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using FonTech.Domain.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FonTech.Consumer;

public class RabbitMqListener(IOptions<RabbitMqSettings> options) : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;

    /// <summary>
    /// Асинхронная инициализация подключения и канала.
    /// </summary>
    private async Task InitializeAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await _channel.QueueDeclareAsync(
            queue: options.Value.QueueName,
            durable: true, // если тру то очередь сохраняет свое состояние и когда случается сбой брокера то все сообщения которые были до перезагрузки сохраняются
            exclusive: false, // если тру то очереди разрешается только одно подключение консьюмера
            autoDelete: false, // если тру то если к очереди никто не подключён то очередь будет удалена
            // если кто то подключен то очередь будет жить,
            arguments: null,
            cancellationToken: ct);

        Console.WriteLine("RabbitMQ Listener initialized.");
    }

    /// <summary>
    /// Основной метод обработки сообщений.
    ///
    /// Работает всё время пока работает приложение - по сути это бэкграунд джоба
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await InitializeAsync(ct);

        if (_channel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += (_, basicDeliver) =>
            {
                var body = basicDeliver.Body.ToArray();
                
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Получено собщение: {message}");

                // Обработка сообщения -- механизм акка --
                // подтверждение получения сообщения чтобы удалить сообщение из очереди
                _channel?.BasicAckAsync(
                    deliveryTag: basicDeliver.DeliveryTag,
                    multiple: false,
                    ct);

                
                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(
                queue: options.Value.QueueName,
                autoAck: false, // не нужно автоматическое подтверждение сообщения
                consumer: consumer,
                ct);
            
        }

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(1000, ct); // Ожидание новых сообщений
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
        base.Dispose();
    }
}