using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;
using IdleOrderService.Infra.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddScoped<IEventStore, EfEventStore>();
        return services;
    }
    
    public static IServiceCollection DecorateAllEventHandlersWithLogging(this IServiceCollection services)
    {
        var handlerType = typeof(IEventHandler<>);
        var decoratorType = typeof(LoggingEventHandlerDecorator<>);

        var registrations = services
            .Where(s => s.ServiceType.IsGenericType 
                        && s.ServiceType.GetGenericTypeDefinition() == handlerType)
            .ToList();

        foreach (var registration in registrations)
        {
            var genericArg = registration.ServiceType.GetGenericArguments()[0];

            var innerType = registration.ImplementationType
                            ?? registration.ImplementationInstance?.GetType()
                            ?? registration.ImplementationFactory?.GetType().GenericTypeArguments.Last()
                            ?? throw new InvalidOperationException("Could not determine implementation type.");

            var decoratedServiceType = handlerType.MakeGenericType(genericArg);
            var decoratorGenericType = decoratorType.MakeGenericType(genericArg);

            services.Decorate(decoratedServiceType, (inner, provider) =>
            {
                var loggerType = typeof(ILogger<>).MakeGenericType(decoratorGenericType);
                var logger = provider.GetRequiredService(loggerType);
                return Activator.CreateInstance(decoratorGenericType, inner, logger)!;
            });
        }

        return services;
    }
    
    public static IServiceCollection DecorateAllEventHandlersWithRetry(this IServiceCollection services)
    {
        var handlerType = typeof(IEventHandler<>);
        var decoratorType = typeof(RetryingEventHandlerDecorator<>);

        var registrations = services
            .Where(s => s.ServiceType.IsGenericType &&
                        s.ServiceType.GetGenericTypeDefinition() == handlerType)
            .ToList();

        foreach (var registration in registrations)
        {
            var genericArg = registration.ServiceType.GetGenericArguments()[0];
            var decoratedServiceType = handlerType.MakeGenericType(genericArg);
            var decoratorGenericType = decoratorType.MakeGenericType(genericArg);

            services.Decorate(decoratedServiceType, (inner, provider) =>
            {
                var loggerType = typeof(ILogger<>).MakeGenericType(decoratorGenericType);
                var logger = provider.GetRequiredService(loggerType);
                return Activator.CreateInstance(decoratorGenericType, inner, logger)!;
            });
        }

        return services;
    }
    
    private static void Decorate(this IServiceCollection services, Type serviceType, Func<object, IServiceProvider, object> decoratorFactory)
    {
        var originalDescriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);
        if (originalDescriptor == null)
            return;

        services.Remove(originalDescriptor);

        services.Add(new ServiceDescriptor(serviceType,
            provider =>
            {
                var inner = originalDescriptor.ImplementationFactory != null
                    ? originalDescriptor.ImplementationFactory(provider)
                    : ActivatorUtilities.CreateInstance(provider, originalDescriptor.ImplementationType!);
                return decoratorFactory(inner, provider);
            },
            originalDescriptor.Lifetime
        ));
    }
}