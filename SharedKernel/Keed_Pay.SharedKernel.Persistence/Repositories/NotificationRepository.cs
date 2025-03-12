using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications.Repositories;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Repositories;

internal sealed class NotificationRepository<TDbContext>(TDbContext dbContext) 
        : INotificationRepository
        where TDbContext : DbContext
{
    public async Task AddAsync(Notification notification)
        => await dbContext.Set<Notification>().AddAsync(notification);
}