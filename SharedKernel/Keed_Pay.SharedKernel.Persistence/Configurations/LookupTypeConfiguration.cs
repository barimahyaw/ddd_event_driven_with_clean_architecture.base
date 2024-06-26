﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Lookups;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

internal class LookupTypeConfiguration<T>(T project) : IEntityTypeConfiguration<LookupType>
     where T : IProjectStringValue
{
    public T Project { get; } = project;

    public void Configure(EntityTypeBuilder<LookupType> builder)
    {
        builder.ToTable("lookup_types", Project.Name);

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
