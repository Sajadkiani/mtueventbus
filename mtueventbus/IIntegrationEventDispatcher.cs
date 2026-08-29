namespace MtuEventBus;

public interface IIntegrationEventDispatcher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}