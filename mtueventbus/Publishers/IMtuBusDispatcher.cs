namespace MtuEventBus.Publishers;

public interface IMtuBusDispatcher
{
    Task PublishAsync(object message, CancellationToken cancellationToken = default);
}