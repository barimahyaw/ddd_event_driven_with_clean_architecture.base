using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Services;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audit.Contexts;

public class AuditableContext<T>(DbContextOptions options, ICurrentUserService currentUserService, ILogger<T> logger)
    : DbContext(options) where T : DbContext
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<T> _logger = logger;

    public DbSet<Audit> AuditTrail { get; set; } = null!;

    public virtual async Task<int> SaveChangesAsync(Guid userId)
    {
        var auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        await OnAfterSaveChanges(auditEntries);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(Guid userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        {
            AuditTrail.Add(auditEntry.ToAudit());
        }
        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue!;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue!;
                }
            }
            AuditTrail.Add(auditEntry.ToAudit());
        }
        return SaveChangesAsync();
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
