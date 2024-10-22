using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;

internal class NotificationConfiguration<P>(P project) : IEntityTypeConfiguration<Notification> 
    where P : IProjectStringValue
{
    public P Project { get; } = project;

    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications", Project.Name);
        builder.Property((Notification x) => x.NotificationType).HasConversion<string>().IsRequired();
    }
}
