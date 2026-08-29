using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MtuEventBus.Consumers;
using MtuEventBus.Options;
using RabbitMQ.Client;

namespace MtuEventBus;

public sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private readonly IMtuBusConnectionManager _connectionManager;
    private readonly MtuRabbitMqOptions _options;
    private readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(
        IMtuBusConnectionManager connectionManager,
        IOptionsMonitor<MtuRabbitMqOptions> optionsMonitor,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _connectionManager = connectionManager;
        _options = optionsMonitor.CurrentValue;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var routeKey = MtuEventBusNameFormatter.ToRoutingKey<T>();
        try
        {
            // I'm sure i will get one connection per app lifetime
            var connection = await _connectionManager.GetConnectionAsync();
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties { Persistent = true };
            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            string basicReturn = null;
            
            channel.BasicReturnAsync =  
            
            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routeKey,
                mandatory: true,
                basicProperties: props,
                body: body, cancellationToken);

            _logger.LogInformation("Published integration event {EventType} to {RouteKey}", typeof(T).Name, routeKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing to RabbitMQ queue {routeKey}.");
            throw;
        }
    }
}