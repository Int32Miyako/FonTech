namespace FonTech.Domain.Interfaces.Producer;

public interface IMessageProducer
{
    // routingKey говорит какой exchange будет связан с роутинг кеем
    void SendMessage<T>(T message, string routingKey, string? exchange = default);
}