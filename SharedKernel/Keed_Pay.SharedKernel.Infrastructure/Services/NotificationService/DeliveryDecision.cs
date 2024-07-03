namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.Services.NotificationService;

public class DeliveryDecision
{
    public bool ShouldDeliver { get; set; }
    public string Reason { get; set; } = default!;
}