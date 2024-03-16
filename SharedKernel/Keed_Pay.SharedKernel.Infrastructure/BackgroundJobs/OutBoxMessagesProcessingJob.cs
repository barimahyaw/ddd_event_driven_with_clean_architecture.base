using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives.Factory;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;
using Keed_Digital.SharedKernel.Infrastructure.Resilience;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Keed_Digital.SharedKernel.Infrastructure.BackgroundJobs;

public class OutBoxMessagesProcessingJob<T>(
    ILogger<OutBoxMessagesProcessingJob<T>> logger,
    T dbContext,
    IPublisher publisher) : IOutBoxMessagesProcessingJob
    where T : DbContext
{
    private readonly ILogger<OutBoxMessagesProcessingJob<T>> _logger = logger;
    private readonly T _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext
               .Set<OutboxMessage>()
               .Where(m => m.ProcessedDateUtc == null)
               .OrderByDescending(m => m.ProcessedDateUtc)
               .Take(20)
               .ToListAsync(cancellationToken);

        if (messages.Count > 0) _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            _logger.LogInformation("Processing outbox message with ID: {Id} of TYPE: {Type}", message.Id, message.Type);

            var errorMessage = $"Failed to process outbox message with ID: {message.Id} of TYPE: {message.Type}";

            var retryPolicy = PollyPolicy<OutBoxMessagesProcessingJob<T>>.Retry(_logger, errorMessage);

            message.ProcessingAttempts++;
            message.ProcessLastAttemptOnUtc = DateTime.UtcNow;

            PolicyResult policyResult = await retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                IDomainEvent domainEvent = EventFactory.CreateEventTypeUsingReflection(message.Assembly, message.Type, message.Content);

                if (domainEvent == null)
                {
                    message.ProcessedDateUtc = DateTime.UtcNow;
                    message.Error = "Failed to deserialize domain event";
                    _logger.LogError("Failed to deserialize domain event for outbox message {Id} of TYPE: {Type}", message.Id, message.Type);
                    return;
                }

                await _publisher.Publish(domainEvent, cancellationToken);

                message.ProcessedDateUtc = DateTime.UtcNow;
                _logger.LogInformation("Successfully processed outbox message with ID: {Id} of TYPE: {Type}", message.Id, message.Type);
            });
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

