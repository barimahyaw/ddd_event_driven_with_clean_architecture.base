using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;

public class AuditableIdentityDbContext<TUser, TRole, TKey>(DbContextOptions options)
    : IdentityDbContext<TUser, TRole, TKey>(options)
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    public DbSet<Audit> AuditTrail { get; set; } = null!;

    public virtual async Task<int> SaveChangesAsync(Ulid userId)
    {
        var auditEntries = AuditHelper.OnBeforeSaveChanges(ChangeTracker, userId, AuditTrail);
        var result = await base.SaveChangesAsync();
        await AuditHelper.OnAfterSaveChanges(ChangeTracker, auditEntries, AuditTrail);
        return result;
    }
}
