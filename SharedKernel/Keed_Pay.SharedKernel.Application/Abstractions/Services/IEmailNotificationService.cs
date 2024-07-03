namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;

public interface IEmailNotificationService
{
    Task SendEmailAsync(
        string email,
        string subject,
        string message,
        byte[] attachment = default!,
        string attachmentName = default!);

    Task SendEmailInBackgroundAsync(
        string email,
        string subject,
        string message,
        byte[] attachment = default!,
        string attachmentName = default!);
}
