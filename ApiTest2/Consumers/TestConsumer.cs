using MtuEventBus.Consumers;

namespace MtuSubscriber.Consumers;

public class TestConsumer : MtuConsumer<TestIntegrationEvent>
{
    private readonly ILogger<TestConsumer> _logger;
    
    public TestConsumer(ILogger<TestConsumer> logger) 
    {
        _logger = logger;
    }

    protected override async Task HandleEventAsync(TestIntegrationEvent message, CancellationToken cancellationToken)
    {
        //TODO: uncomment this for thes DLQ
        //throw new NotImplementedException();
        
        _logger.LogInformation($"Received event id {message.EventId}, name {typeof(TestIntegrationEvent).FullName} queue {QueueName}");

        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(TestIntegrationEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event id {message.EventId} added to inbox.");
        return Task.CompletedTask;
    }
}