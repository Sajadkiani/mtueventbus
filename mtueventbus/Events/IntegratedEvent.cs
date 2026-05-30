namespace MtuEventBus.Events;

public class IntegratedEvent : IEvent
{
    public Guid EventId { get; private set; }
    public Guid? CorrelationId { get; private set; }
    public string? NationalId { get; private set; }
    public DateTime PublishDateTime { get; private set; }
    

    public IntegratedEvent(Guid eventId, DateTime publishDateTime, Guid? correlationId = null, string? nationalId = null)
    {
        this.EventId = eventId;
        this.PublishDateTime = publishDateTime;
        CorrelationId = correlationId;
        NationalId = nationalId;
    }

    public void SetCorrelation(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
}