namespace Keed_Digital.SharedKernel.Infrastructure.BackgroundJobs;

public interface IOutBoxMessagesProcessingJob
{
    Task Execute(CancellationToken cancellationToken);
}