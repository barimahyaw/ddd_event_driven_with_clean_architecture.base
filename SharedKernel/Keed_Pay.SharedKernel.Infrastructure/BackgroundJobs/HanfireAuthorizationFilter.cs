using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Keed_Digital.SharedKernel.Infrastructure.BackgroundJobs;

public sealed class HanfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
