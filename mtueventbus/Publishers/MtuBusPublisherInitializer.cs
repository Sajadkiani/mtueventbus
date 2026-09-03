using Microsoft.Extensions.Options;
using MtuEventBus.Consumers;
using MtuEventBus.Options;
using RabbitMQ.Client;

namespace MtuEventBus;

public sealed class MtuBusPublisherInitializer
{
    private readonly MtuRabbitMqOptions _options;
    private readonly IMtuBusChannelManager _channelManager;

    public MtuBusPublisherInitializer(
        IOptionsMonitor<MtuRabbitMqOptions> optionsMonitor
        , IMtuBusChannelManager channelManager)
    {
        _options = optionsMonitor.CurrentValue;
        _channelManager = channelManager;
    }

    public async Task Initialize(CancellationToken cancellationToken)
    {
        var channel = await _channelManager.GetChannelAsync(cancellationToken);

        await DeclareExchangeAsync(channel, cancellationToken);
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
