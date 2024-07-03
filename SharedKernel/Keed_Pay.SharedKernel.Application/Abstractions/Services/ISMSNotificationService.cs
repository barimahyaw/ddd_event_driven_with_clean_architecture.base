namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;

public interface ISMSNotificationService
{
    Task SendSMSAsync(string[] to, string message);
    Task SendSMSInBackgroundAsync(string[] to, string message);
}