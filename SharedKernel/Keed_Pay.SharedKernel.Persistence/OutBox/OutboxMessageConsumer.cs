namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;

public class OutboxMessageConsumer
{
    public Ulid Id { get; set; }
    public string Name { get; set; } = null!;
}