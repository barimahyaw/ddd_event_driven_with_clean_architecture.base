using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives.Factory;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;
using Keed_Digital.SharedKernel.Infrastructure.Resilience;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Keed_Digital.SharedKernel.Infrastructure.BackgroundJobs;

public class OutBoxMessagesProcessingJob<P>(
    ILogger<OutBoxMessagesProcessingJob<P>> logger,
    SharedDbContext<P> dbContext,
    IPublisher publisher) : IOutBoxMessagesProcessingJob
    where P : IProjectStringValue
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var messages = await dbContext
               .Set<OutboxMessage>()
               .Where(m => m.ProcessedDateUtc == null)
               .OrderByDescending(m => m.ProcessedDateUtc)
               .Take(20)
               .ToListAsync(cancellationToken);

        if (messages.Count > 0) logger.LogInformation("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            logger.LogInformation("Processing outbox message with ID: {Id} of TYPE: {Type}", message.Id, message.Type);

            var errorMessage = $"Failed to process outbox message with ID: {message.Id} of TYPE: {message.Type}";

            var retryPolicy = PollyPolicy<OutBoxMessagesProcessingJob<P>>.Retry(logger, errorMessage);

            message.ProcessingAttempts++;
            message.ProcessLastAttemptOnUtc = DateTime.UtcNow;

            PolicyResult policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                IDomainEvent domainEvent = EventFactory.CreateEventTypeUsingReflection(message.Assembly, message.Type, message.Content);

                if (domainEvent == null)
                {
                    message.ProcessedDateUtc = DateTime.UtcNow;
                    message.Error = "Failed to deserialize domain event";
                    logger.LogError("Failed to deserialize domain event for outbox message {Id} of TYPE: {Type}", message.Id, message.Type);
                    return;
                }

                await publisher.Publish(domainEvent, cancellationToken);

                message.ProcessedDateUtc = DateTime.UtcNow;
                logger.LogInformation("Successfully processed outbox message with ID: {Id} of TYPE: {Type}", message.Id, message.Type);
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}