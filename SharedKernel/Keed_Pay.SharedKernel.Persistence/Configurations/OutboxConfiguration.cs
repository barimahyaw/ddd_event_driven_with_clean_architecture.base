using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

public class OutboxConfiguration<T>(T project) : IEntityTypeConfiguration<OutboxMessage>
    where T : IProjectStringValue
{
    public T Project { get; } = project;

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", Project.Name);
    }
}