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
    private readonly MtuRabbitMqOptions _options;
    private readonly IMtuBusChannelManager _channelManager;
    private readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(
        IMtuBusChannelManager channelManager,
        IOptionsMonitor<MtuRabbitMqOptions> optionsMonitor,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _channelManager = channelManager;
        _logger = logger;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        try
        {
            var type = message.GetType();
            var channel = await _channelManager.GetChannelAsync(cancellationToken);

            await DeclareExchangeAsync(channel, cancellationToken);

            var returnReason = SetBasicReturnHandlerAsync(channel);
            
            await PublishAsync(message, cancellationToken, channel);

            CheckIfMessageRouted(type, returnReason);

            _logger.LogInformation("Published integration event {EventType} to {RouteKey}",
                message.GetType().Name,
                MtuEventBusNameFormatter.ToRoutingKey(type));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing to RabbitMQ queue {MtuEventBusNameFormatter.ToRoutingKey(message.GetType())}.");
            throw;
        }
    }

    private static string? SetBasicReturnHandlerAsync(IChannel channel)
    {
        string returnReason = null;
        channel.BasicReturnAsync += (obj,args) =>
        {
            returnReason = $"replyCode:{args.ReplyCode} replyText{args.ReplyText} routingKey:{args.RoutingKey}";
            return Task.CompletedTask;
        };
        
        return returnReason;
    }

    private static void CheckIfMessageRouted(Type type, string? returnReason)
    {
        string routeKey = MtuEventBusNameFormatter.ToRoutingKey(type);
        if (returnReason is not null)
            throw new InvalidOperationException($"Message unroutable, routingKey='{routeKey}': {returnReason}");
    }

    private async Task PublishAsync(object message, CancellationToken cancellationToken, IChannel channel)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        var props = new BasicProperties { Persistent = true };
            
        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: MtuEventBusNameFormatter.ToRoutingKey(message.GetType()),
            mandatory: true,
            basicProperties: props,
            body: body, cancellationToken);
    }

    private async Task DeclareExchangeAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }
}