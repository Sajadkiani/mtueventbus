namespace MtuSubscriber;

public class Test2IntegrationEvent
{
    public Test2IntegrationEvent(string test2IntegrationEventProperty, Guid eventId, DateTime createdOn,
        Guid? correlationId = null)
    {
        Test2IntegrationEventProperty = test2IntegrationEventProperty;
    }

    public string Test2IntegrationEventProperty { get; init; }
}

public class TestIntegrationEvent
{
    public TestIntegrationEvent(string testIntegrationEventProperty, Guid eventId,
        DateTime createdOn, Guid? correlationId = null
    )
    {
        TestIntegrationEventProperty = testIntegrationEventProperty;
    }

    public string TestIntegrationEventProperty { get; init; }
}