using RabbitMQ.Client;

namespace MtuEventBus.Consumers;

public interface IMtuBusConnectionManager
{
    Task<IConnection> GetConnectionAsync();
}