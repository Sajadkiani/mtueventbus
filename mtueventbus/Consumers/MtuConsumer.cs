using Newtonsoft.Json;

namespace MtuEventBus.Consumers;

public abstract class MtuConsumer
{
    public string QueueName { get; }
    public string RoutingKey { get; }

    protected MtuConsumer(Type type)
    {
        RoutingKey = MtuEventBusNameFormatter.ToRoutingKey<Type>();
        QueueName = MtuEventBusNameFormatter.ToQueueName(type);
    }
    
    public abstract Task HandleAsync(string json, CancellationToken cancellationToken);
}

public abstract class MtuConsumer<TMessage> : MtuConsumer where TMessage : class 
{
    protected MtuConsumer() : base(typeof(TMessage))
    {
    }

    protected abstract Task AddReceivedEventAsync(TMessage message, CancellationToken cancellationToken);
    
    public override async Task HandleAsync(string json, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON message is null or empty.", nameof(json));

        var message = JsonConvert.DeserializeObject<TMessage>(json);
        if (message == null)
            throw new InvalidOperationException($"Unable to deserialize message as {typeof(TMessage).Name}.");

        await AddReceivedEventAsync(message, cancellationToken);

        await HandleEventAsync(message, cancellationToken);
    }

    protected abstract Task HandleEventAsync(TMessage message, CancellationToken cancellationToken);
}