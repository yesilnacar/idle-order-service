using IdleOrderService.Core.Event;

namespace IdleOrderService.Domain.Event;

public interface IEventStore
{
    Task SaveAsync(IEvent @event, CancellationToken cancellationToken);
}