using MtuEventBus.Consumers;

namespace MtuSubscriber.Consumers;

public class TestIdempotencyConsumer<T> : MtuConsumer<T> where T : class
{
    private readonly  ILogger<TestIdempotencyConsumer<T>> _logger;
    public TestIdempotencyConsumer(ILogger<TestIdempotencyConsumer<T>> logger)
    {
        _logger = logger;
    }
    
    protected override Task AddReceivedEventAsync(T message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventAsync(T message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"idempotency consumer ran.");
        return Task.CompletedTask;
    }
}

public class TestConsumer : TestIdempotencyConsumer<TestIntegrationEvent>
{
    private readonly ILogger<TestConsumer> _logger;
    
    public TestConsumer(ILogger<TestConsumer> logger) : base(logger)
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