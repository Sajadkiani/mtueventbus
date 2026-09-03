namespace MtuEventBus;

public interface IMtuBusDispatcher
{
    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}