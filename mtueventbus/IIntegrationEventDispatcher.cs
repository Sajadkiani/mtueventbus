namespace MtuEventBus;

public interface IIntegrationEventDispatcher
{
    Task PublishAsync<T>(string routeKey, T message, CancellationToken cancellationToken = default);
}