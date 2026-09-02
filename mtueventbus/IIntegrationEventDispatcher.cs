namespace MtuEventBus;

public interface IIntegrationEventDispatcher
{
    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}