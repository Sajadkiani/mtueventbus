using MtuEventBus.Consumers;
using MtuEventBus.Events;

namespace MtuSubscriber.Consumers;

public class Test2Consumer : MtuConsumer<Test2IntegrationEvent>
{
    private readonly ILogger<Test2Consumer> _logger;
    
    public Test2Consumer(ILogger<Test2Consumer> logger) 
    {
        _logger = logger;
    }

    protected override async Task HandleEventAsync(Test2IntegrationEvent message, CancellationToken cancellationToken)
    {
        //TODO: uncomment this for thes DLQ
        throw new NotImplementedException();
        
        _logger.LogInformation($"Received event id {message.EventId}, name {typeof(Test2IntegrationEvent).FullName} queue {QueueName}");

        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(Test2IntegrationEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event id {message.EventId} added to inbox.");
        return Task.CompletedTask;
    }
}