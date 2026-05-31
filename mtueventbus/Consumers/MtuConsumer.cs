using MtuEventBus.Events;
using Newtonsoft.Json;

namespace MtuEventBus.Consumers;

public abstract class MtuConsumer
{
    public string QueueName { get; protected set; } = default!;
    public string RoutingKey { get; protected set; } = default!;
    
    protected MtuConsumer()
    {
    }

    protected abstract Task HandleEventAsync(IntegratedEvent message, CancellationToken cancellationToken);
    
    protected abstract Task AddReceivedEventAsync(IntegratedEvent message, CancellationToken cancellationToken);

    public async Task HandleAsync(string json, CancellationToken cancellationToken)
    {
        if (json == null)
            throw new NullReferenceException("json message is null");

        var message = JsonConvert.DeserializeObject<IntegratedEvent>(json);
        if (message == null)
            throw new NullReferenceException("message is null");

        // await _context.AppReceivedEvents.AddAsync(new AppReceivedEvent(message.EventId, message.FullName, json),
        //     cancellationToken);
        
        await AddReceivedEventAsync(message, cancellationToken);

        
        await HandleEventAsync(message, cancellationToken);
    }
}