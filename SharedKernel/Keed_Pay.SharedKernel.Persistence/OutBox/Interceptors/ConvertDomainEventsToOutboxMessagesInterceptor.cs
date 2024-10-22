using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor<P>(SharedDbContext<P> sharedDbContext) 
    : SaveChangesInterceptor
    where P : IProjectStringValue
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents().ToList();
                aggregateRoot.ClearDomainEvents();

                return domainEvents;

            })
            .Select(domainEvent => new OutboxMessage
            {
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().FullName!,
                Assembly = domainEvent.GetType().Assembly.FullName!,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            })
            .ToList();

        sharedDbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        sharedDbContext.SaveChangesAsync(cancellationToken);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
