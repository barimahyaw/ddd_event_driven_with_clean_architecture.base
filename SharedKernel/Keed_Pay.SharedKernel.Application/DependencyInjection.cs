using FluentValidation;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMediatorConfig<T>(this IServiceCollection services)
        where T : class
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<T>();

            config.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(UnitOfWorkPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        return services;
    }

    public static IServiceCollection AddAssemblyValidator<T>(this IServiceCollection services)
        where T : class
    {
        services.AddValidatorsFromAssembly(typeof(T).Assembly, includeInternalTypes: true);
        return services;
    }
}
