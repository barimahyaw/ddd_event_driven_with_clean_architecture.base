﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications.Repositories;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Notifications;

internal sealed class NotificationRepository<P>(SharedDbContext<P> dbContext) 
    : INotificationRepository
    where P : IProjectStringValue
{
    public async Task AddAsync(Notification notification)
        => await dbContext.Set<Notification>().AddAsync(notification);
}