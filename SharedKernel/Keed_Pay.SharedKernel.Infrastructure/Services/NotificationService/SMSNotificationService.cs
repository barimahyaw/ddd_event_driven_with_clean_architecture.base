using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;
using Hangfire;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.Services.NotificationService;

internal class SMSNotificationService : ISMSNotificationService
{
    public Task SendSMSAsync(string[] to, string message)
    {
        throw new NotImplementedException();
    }

    public Task SendSMSInBackgroundAsync(string[] to, string message) 
        => Task.FromResult(BackgroundJob.Enqueue(() => SendSMSAsync(to, message)));
}