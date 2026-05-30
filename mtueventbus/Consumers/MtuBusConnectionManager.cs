using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MtuEventBus.Options;
using RabbitMQ.Client;

namespace MtuEventBus.Consumers;

public class MtuBusConnectionManager : IMtuBusConnectionManager, IDisposable
{
    private readonly MtuRabbitMqOptions _options;
    private readonly ILogger<MtuBusConnectionManager> _logger;
    private IConnection? _connection;

    public MtuBusConnectionManager(
        IOptions<MtuRabbitMqOptions> options,
        ILogger<MtuBusConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// get an connection if exist old one or create new one
    /// </summary>
    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
        };

        _logger.LogInformation($"Creating new MTU bus connection to {_options.HostName}.");
        _connection = await factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        if (_connection is { IsOpen: true })
        {
            _logger.LogInformation("Closing MTU bus connection...");
            _connection.CloseAsync().GetAwaiter().GetResult();
            _connection.Dispose();
        }
    }
}