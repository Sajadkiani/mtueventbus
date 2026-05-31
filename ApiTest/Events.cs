using MtuEventBus.Events;

namespace ApiTest;

public class Test2IntegrationEvent : IntegratedEvent
{
    public Test2IntegrationEvent(string test2IntegrationEventProperty, Guid eventId, DateTime createdOn,
        Guid? correlationId = null) : base(eventId, createdOn, correlationId)
    {
        Test2IntegrationEventProperty = test2IntegrationEventProperty;
    }

    public string Test2IntegrationEventProperty { get; init; }
}

public class TestIntegrationEvent : IntegratedEvent
{
    public TestIntegrationEvent(string testIntegrationEventProperty, Guid eventId,
        DateTime createdOn, Guid? correlationId = null
    ) : base(eventId, createdOn, correlationId)
    {
        TestIntegrationEventProperty = testIntegrationEventProperty;
    }

    public string TestIntegrationEventProperty { get; init; }
}