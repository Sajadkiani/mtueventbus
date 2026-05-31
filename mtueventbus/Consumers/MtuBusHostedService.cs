using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MtuEventBus.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace MtuEventBus.Consumers;

public class MtuBusHostedService : BackgroundService
{
    private readonly ILogger<MtuBusHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMtuBusConnectionManager _connectionManager;
    private readonly MtuRabbitMqOptions _options;

    private readonly List<IChannel> _channels = new();

    public MtuBusHostedService(
        IMtuBusConnectionManager connectionManager,
        ILogger<MtuBusHostedService> logger,
        IServiceProvider serviceProvider,
        IOptions<MtuRabbitMqOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connectionManager = connectionManager;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connection =
                await _connectionManager.GetConnectionAsync();
            
            using var startupScope =
                _serviceProvider.CreateScope();

            var consumers =
                startupScope.ServiceProvider
                    .GetServices<MtuConsumer>()
                    .Distinct()
                    .ToList();

            foreach (var consumer in consumers)
            {
                var channel =
                    await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                _channels.Add(channel);

                var deadLetterExchange = $"{_options.ExchangeName}.dlx";
                var deadLetterQueue = $"{consumer.QueueName}.dlq";

                await channel.ExchangeDeclareAsync(
                    deadLetterExchange,
                    ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueDeclareAsync(
                    deadLetterQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: deadLetterQueue, 
                    exchange: deadLetterExchange,
                    routingKey: deadLetterQueue, //cuz the exchange is direct
                    cancellationToken: cancellationToken);
                
                // Main queue arguments
                var queueArguments = new Dictionary<string, object?>
                {
                    ["x-dead-letter-exchange"] = deadLetterExchange,
                    ["x-dead-letter-routing-key"] = deadLetterQueue
                };
                
                // EXCHANGE
                await channel.ExchangeDeclareAsync(
                    exchange: _options.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                // QUEUE
                await channel.QueueDeclareAsync(
                    queue: consumer.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: queueArguments,
                    cancellationToken: cancellationToken);

                // BINDING
                await channel.QueueBindAsync(
                    queue: consumer.QueueName,
                    exchange: _options.ExchangeName,
                    routingKey: consumer.RoutingKey,
                    cancellationToken: cancellationToken);

                // QOS
                await channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: 1,
                    global: false,
                    cancellationToken: cancellationToken);

                var rabbitConsumer = new AsyncEventingBasicConsumer(channel);

                rabbitConsumer.ReceivedAsync += async (_, ea) =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    try
                    {
                        _logger.LogInformation("Received event {RoutingKey}", ea.RoutingKey);

                        await consumer.HandleAsync(json, cancellationToken);

                        await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing event {RoutingKey}", ea.RoutingKey);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: consumer.QueueName,
                    autoAck: false,
                    consumer: rabbitConsumer,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Consumer started. Queue= {Queue} RoutingKey= {RoutingKey}",
                    consumer.QueueName,
                    consumer.RoutingKey);

            }

            _logger.LogInformation(
                "MTU bus started with {Count} consumers",
                consumers.Count);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Broker unreachable");
        }
    }
}