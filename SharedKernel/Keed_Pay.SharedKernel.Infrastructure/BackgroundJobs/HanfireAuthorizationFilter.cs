using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Infrastructure.BackgroundJobs;

public sealed class HanfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}
