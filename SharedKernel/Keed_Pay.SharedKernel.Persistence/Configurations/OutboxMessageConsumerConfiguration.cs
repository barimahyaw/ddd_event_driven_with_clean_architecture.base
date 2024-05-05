using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

public class OutboxMessageConsumerConfiguration<T>(T project) : IEntityTypeConfiguration<OutboxMessageConsumer>
    where T : IProjectStringValue
{
    public T Project { get; } = project;
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable("outbox_messages_consumer", Project.Name);
    }
}