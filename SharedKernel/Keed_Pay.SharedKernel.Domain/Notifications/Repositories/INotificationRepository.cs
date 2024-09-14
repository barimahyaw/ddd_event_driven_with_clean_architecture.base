namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
}