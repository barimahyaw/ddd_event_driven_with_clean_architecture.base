using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Lookups;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

internal class LookupValueConfiguration<T>(T project) : IEntityTypeConfiguration<LookupValue>
    where T : IProjectStringValue
{
    public T Project { get; } = project;

    public void Configure(EntityTypeBuilder<LookupValue> builder)
    {
        builder.ToTable("lookup_values", Project.Name);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ValueName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.ValueDescription)
            .HasMaxLength(255)
            .IsRequired();

        EntityExtraConfiguration<LookupValue>.Configure(builder);
    }
}