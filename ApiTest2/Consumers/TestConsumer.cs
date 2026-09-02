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
        
        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(TestIntegrationEvent message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}