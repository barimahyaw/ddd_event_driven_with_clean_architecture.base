using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Keed_Digital.SharedKernel.Infrastructure.Resilience;

public class PollyPolicy<T> where T : class
{
    public static AsyncRetryPolicy Retry(ILogger<T> logger, string failDesc)
    {
        AsyncRetryPolicy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromMilliseconds(50 * retryAttempt),
                (exception, timeSpan, context) =>
                {
                    // Log the exception here
                    logger.LogError(exception, failDesc);
                });

        return policy;
    }
}