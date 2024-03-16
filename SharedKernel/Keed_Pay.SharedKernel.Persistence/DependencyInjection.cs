using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.OutBox.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence;

public static class DependencyInjection
{
    private static DbContextOptionsBuilder ConfigureDbContextOptions(
        this DbContextOptionsBuilder options,
        IConfiguration configuration)
        => options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? configuration.GetConnectionString("CONNECTION_STRING"));

    public static IServiceCollection AddDatabaseConfiguration<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<TDbContext>(
            (sp, optionBuilder) =>
            {
                optionBuilder.ConfigureDbContextOptions(configuration);

                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();

                optionBuilder.AddInterceptors(interceptor!);
            });

        return services;
    }

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
}
