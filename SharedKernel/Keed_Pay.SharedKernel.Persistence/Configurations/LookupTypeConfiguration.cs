﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Lookups;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

internal class LookupTypeConfiguration<S>(S schema) : IEntityTypeConfiguration<LookupType>
     where S : ISchemaStringValue
{
    public S Schema { get; } = schema;

    public void Configure(EntityTypeBuilder<LookupType> builder)
    {
        builder.ToTable("lookup_types", Schema.Name);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TypeName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasMany(c => c.LookupValues)
            .WithOne()
            .HasForeignKey(c => c.LookupTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        EntityExtraConfiguration<LookupType>.Configure(builder);
    }
}
