using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications.Repositories;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits.Contexts;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Notifications;

internal sealed class NotificationRepository<T>(T dbContext) 
    : INotificationRepository
    where T : AuditableContext<T>    
{
    private readonly T _dbContext = dbContext;

    public async Task AddAsync(Notification notification)
        => await _dbContext.Set<Notification>().AddAsync(notification);
}