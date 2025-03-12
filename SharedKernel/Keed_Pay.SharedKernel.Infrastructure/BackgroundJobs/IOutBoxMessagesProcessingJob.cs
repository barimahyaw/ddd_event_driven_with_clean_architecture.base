namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.BackgroundJobs;

public interface IOutBoxMessagesProcessingJob
{
    Task Execute(CancellationToken cancellationToken);
}