using RabbitMQ.Client;

namespace MtuEventBus.Consumers;

public interface IMtuBusChannelManager
{
    Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default);
}

public class MtuBusChannelManager : IMtuBusChannelManager
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
        try
        {
            if (_channel is { IsOpen: true })
                return _channel;
            
            await _semaphore.WaitAsync(cancellationToken);
            
            if (_channel is { IsOpen: true })
                return _channel;
            
            var connection = await _mtuBusConnectionManager.GetConnectionAsync(cancellationToken);
            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            
            return _channel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}