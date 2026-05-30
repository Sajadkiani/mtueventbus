public class MtuConsumerRegistration
{
    public Type ConsumerType { get; set; } = default!;

    public string QueueName { get; set; } = default!;

    public string RoutingKey { get; set; } = default!;
}