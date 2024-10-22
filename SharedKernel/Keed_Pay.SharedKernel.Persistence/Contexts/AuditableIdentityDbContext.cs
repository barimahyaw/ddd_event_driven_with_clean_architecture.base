using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;

public class AuditableIdentityDbContext<TUser, TRole, TKey, P>(DbContextOptions options, SharedDbContext<P> sharedDbContext)
    : IdentityDbContext<TUser, TRole, TKey>(options)
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where P : IProjectStringValue
{
    public DbSet<Audit> AuditTrail { get; set; } = null!;

    public virtual async Task<int> SaveChangesAsync(Guid userId)
    {
        var auditEntries = sharedDbContext.OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        await sharedDbContext.OnAfterSaveChanges(auditEntries);
        return result;
    }
}
