namespace MtuEventBus.Events;

public interface IEvent
{
    Guid EventId { get; }
}