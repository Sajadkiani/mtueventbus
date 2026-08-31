using RabbitMQ.Client;

namespace MtuEventBus.Consumers;

public interface IMtuBusChannelManager
{
    Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default);
}

public class MtuBusChannelManager : IMtuBusChannelManager, IAsyncDisposable
{
    private readonly IMtuBusConnectionManager _mtuBusConnectionManager;
    private readonly SemaphoreSlim _semaphore = new (1, 1);
    private IChannel _channel;
    public MtuBusChannelManager(IMtuBusConnectionManager mtuBusConnectionManager)
    {
        _mtuBusConnectionManager = mtuBusConnectionManager;
    }
    
    public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {       
            if (_channel is { IsOpen: true })
                return _channel;
            
            var connection = await _mtuBusConnectionManager.GetConnectionAsync(cancellationToken);
            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            
            return _channel;
        }
        finally
        {
            _semaphore.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is { IsOpen: true })
        {
            await _channel.DisposeAsync();
            _channel = null!;
        }
        
        _semaphore.Dispose();
    }
}