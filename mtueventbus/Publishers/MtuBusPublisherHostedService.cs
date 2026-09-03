using Microsoft.Extensions.Hosting;

namespace MtuEventBus;

public class MtuBusPublisherHostedService : IHostedService
{
    private readonly MtuBusPublisherInitializer _initializer;

    public MtuBusPublisherHostedService(
        MtuBusPublisherInitializer initializer
    )
    {
        _initializer = initializer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _initializer.Initialize(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
