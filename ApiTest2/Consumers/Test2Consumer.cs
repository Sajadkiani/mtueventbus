using MtuEventBus.Consumers;

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
        

        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(Test2IntegrationEvent message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}