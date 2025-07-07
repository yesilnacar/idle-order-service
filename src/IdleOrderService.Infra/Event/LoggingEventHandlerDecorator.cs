using IdleOrderService.Core.Event;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace IdleOrderService.Infra.Event;

public class LoggingEventHandlerDecorator<TEvent>(
    IEventHandler<TEvent> inner,
    ILogger<LoggingEventHandlerDecorator<TEvent>> logger) : IEventHandler<TEvent> where TEvent : IEvent
{
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        var eventName = @event.GetType().Name;
        try
        {
            logger.LogInformation("[Event] Handling event {EventName}", eventName);
            await inner.HandleAsync(@event, cancellationToken);
            logger.LogInformation("[Event] Successfully handled event {EventName}", eventName);
        }
        catch (Exception e)
        {
            logger.LogError("[Event] Error handling event {EventName}: {ErrorMessage}", eventName, e.Message);
            throw;
        }
    }
}