namespace MtuEventBus.Events;

public class IntegratedEvent : IEvent
{
    public Guid EventId { get; private set; }
    public Guid? CorrelationId { get; private set; }
    public DateTime CreatedOn { get; private set; }

    protected IntegratedEvent(Guid eventId, DateTime createdOn, Guid? correlationId = null)
    {
        EventId = eventId;
        CreatedOn = createdOn;
        CorrelationId = correlationId;
    }
}