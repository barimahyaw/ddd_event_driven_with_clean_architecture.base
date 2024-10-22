using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;

public class AuditableContext<T, P>
    (DbContextOptions options, ICurrentUserService currentUserService, ILogger<T> logger, SharedDbContext<P> sharedDbContext)
    : DbContext(options)
    where T : DbContext
    where P : IProjectStringValue
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<T> _logger = logger;

    public DbSet<Audit> AuditTrail { get; set; } = null!;

    public virtual async Task<int> SaveChangesAsync(Guid userId)
    {
        var auditEntries = sharedDbContext.OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        await sharedDbContext.OnAfterSaveChanges(auditEntries);
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var userIdValidationResult = UserId.Validate(_currentUserService.UserId);
        if (!userIdValidationResult.Succeeded)
        {
            _logger.LogError($"Save Changes operation failed because of invalid user Id: {_currentUserService.UserId}");
            return 0;
        }

        var userIdResult = UserId.Create(_currentUserService.UserId);

        foreach (var entry in ChangeTracker.Entries<EntityExtra>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = entry.Entity.CreatedAtUtc != DateTime.MinValue
                        ? entry.Entity.CreatedAtUtc
                        : DateTime.UtcNow;
                    entry.Entity.CreatedUserId = userIdResult;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAtUtc = DateTime.UtcNow;
                    entry.Entity.LastModifiedUserId = userIdResult;
                    break;
            }
        }
        if (_currentUserService.UserId == Guid.Empty)
            return await base.SaveChangesAsync(cancellationToken);
        return await SaveChangesAsync(_currentUserService.UserId);
    }
}
