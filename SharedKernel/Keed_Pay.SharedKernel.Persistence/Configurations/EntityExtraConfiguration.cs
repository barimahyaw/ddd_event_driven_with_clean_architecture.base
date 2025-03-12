using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

public static class EntityExtraConfiguration<T> where T : EntityExtra
{
    public static EntityTypeBuilder Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(c => c.CreatedUserId)
            .HasConversion(
                userId => userId.Id.ToString(),
                value => UserId.Create(Ulid.Parse(value))
            )
            .HasColumnType("varchar(26)");

        builder.Property(c => c.LastModifiedUserId)
            .HasConversion(
                userId => userId != null ? userId.Id.ToString() : null,
                value => value != null ? UserId.Create(Ulid.Parse(value)) : null
            )
            .HasColumnType("varchar(26)");

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        return builder;
    }
}