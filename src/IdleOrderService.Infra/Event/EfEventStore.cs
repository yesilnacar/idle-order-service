using System.Text.Json;
using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;
using IdleOrderService.Infra.Persistence;

namespace IdleOrderService.Infra.Event;

public class EfEventStore(AppDbContext dbContext) : IEventStore
{
    public async Task SaveAsync(IEvent @event, CancellationToken cancellationToken)
    {
        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            Type = @event.GetType().AssemblyQualifiedName ?? "UnknownEventType",
            Payload = JsonSerializer.Serialize(@event, @event.GetType())
        };
        
        dbContext.OutboxEvents.Add(outboxEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}