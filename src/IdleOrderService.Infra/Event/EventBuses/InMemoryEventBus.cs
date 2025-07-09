using IdleOrderService.Core.Event;
using Microsoft.Extensions.DependencyInjection;

namespace IdleOrderService.Infra.Event;

public class InMemoryEventBus(IServiceScopeFactory serviceScopeFactory) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        var eventType = @event.GetType();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        using var scope = serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var handlers = serviceProvider.GetServices(handlerType);
        if (!handlers.Any())
            throw new InvalidOperationException($"No handlers found for event type {eventType.Name}");

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("HandleAsync")!;
            await (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
        }
    }
}