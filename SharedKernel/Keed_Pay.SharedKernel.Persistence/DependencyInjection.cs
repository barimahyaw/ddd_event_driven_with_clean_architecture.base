using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Notifications.Repositories;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Configurations;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Contexts;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Notifications;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence;

public static class DependencyInjection
{
    private static DbContextOptionsBuilder ConfigureDbContextOptions(this DbContextOptionsBuilder options,
        string connectionString)
        => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

    public static IServiceCollection AddDatabaseConfiguration<TDbContext, P>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
        where P : IProjectStringValue
    {
        services.AddScoped<ConvertDomainEventsToOutboxMessagesInterceptor<P>>();

        var cs = Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? configuration.GetConnectionString("CONNECTION_STRING")
            ?? throw new ArgumentNullException("CONNECTION_STRING");

        services.AddDbContext<TDbContext>(
            (sp, optionBuilder) =>
            {                
                optionBuilder.ConfigureDbContextOptions(cs);

                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor<P>>();
                optionBuilder.AddInterceptors(interceptor!);
            });

        return services;
    }

    public static IServiceCollection AddSharedDatabaseConfiguration<P>(this IServiceCollection services, string connectionString)
        where P : IProjectStringValue
        => services.AddDbContext<SharedDbContext<P>>(options => options.ConfigureDbContextOptions(connectionString));

    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services)
        where T : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<T>>();

        return services;
    }

    public static IServiceCollection AddOutboxIdempotentConfig(this IServiceCollection services)
    {
        // the decorator pattern is used to make sure that the domain event handlers are idempotent
        // come from the Scrutor library
        //services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<,>));

        return services;
    }

    public static IServiceCollection AddNotificationRepository<P>(this IServiceCollection services)
        where P : IProjectStringValue
        => services.AddScoped<INotificationRepository, NotificationRepository<P>>();
}