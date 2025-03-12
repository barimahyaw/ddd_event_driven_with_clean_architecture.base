using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

public class OutboxConfiguration<T>(T schema) : IEntityTypeConfiguration<OutboxMessage>
    where T : ISchemaStringValue
{
    public T Schema { get; } = schema;

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", Schema.Name);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.ToString(),
                value => Ulid.Parse(value)
            )
            .HasColumnType("varchar(26)");
    }
}