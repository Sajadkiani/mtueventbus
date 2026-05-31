using MtuEventBus.Consumers;
using MtuEventBus.Events;

namespace ApiTest.Consumers;

public class Test2Consumer : MtuConsumer
{
    private readonly ILogger<Test2Consumer> _logger;
    
    public Test2Consumer(
        ILogger<Test2Consumer> logger) 
    {
        _logger = logger;
        
        //TODO: move these lines to parent class
        RoutingKey = MtuEventBusNameFormatter.ToRoutingKey<Test2IntegrationEvent>();
        QueueName = MtuEventBusNameFormatter.GetQueueName("artwork");
    }

    protected override async Task HandleEventAsync(IntegratedEvent message, CancellationToken cancellationToken)
    {
        //TODO: uncomment this for thes DLQ
        //throw new NotImplementedException();
        
        _logger.LogInformation($"Received event id {message.EventId}, name {typeof(TestIntegrationEvent).FullName} queue {QueueName}");

        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(IntegratedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event id {message.EventId} added to inbox.");
        return Task.CompletedTask;
    }
}

public class TestConsumer : MtuConsumer
{
    private readonly ILogger<TestConsumer> _logger;
    
    public TestConsumer(
        ILogger<TestConsumer> logger) 
    {
        _logger = logger;
        
        //TODO: move these lines to parent class
        RoutingKey = MtuEventBusNameFormatter.ToRoutingKey<TestIntegrationEvent>();
        QueueName = MtuEventBusNameFormatter.GetQueueName("artwork");
    }

    protected override async Task HandleEventAsync(IntegratedEvent message, CancellationToken cancellationToken)
    {
        //TODO: uncomment this for thes DLQ
        //throw new NotImplementedException();
        
        _logger.LogInformation($"Received event id {message.EventId}, name {typeof(TestIntegrationEvent).FullName} queue {QueueName}");

        // await _eventDispatcher.PublishAsync(new TestDomainEvent(testEvent.UserName));
        _logger.LogInformation("Handled event");
    }

    protected override Task AddReceivedEventAsync(IntegratedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event id {message.EventId} added to inbox.");
        return Task.CompletedTask;
    }
}