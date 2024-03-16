namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;

public class OutboxMessageConsumer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}