using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

public class AuditTrailConfiguration<T>(T project) : IEntityTypeConfiguration<Audit>
    where T : IProjectStringValue
{
    public T Project { get; } = project;

    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable("audits", Project.Name);
    }
}