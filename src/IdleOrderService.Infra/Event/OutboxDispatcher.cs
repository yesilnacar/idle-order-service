using System.Text.Json;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event;

public class OutboxDispatcher : BackgroundService
{
    private readonly AppDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OutboxDispatcher> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public OutboxDispatcher(AppDbContext dbContext, IEventBus eventBus, ILogger<OutboxDispatcher> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Dispatcher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var events = await _dbContext.OutboxEvents
                    .Where(x => !x.Processed)
                    .OrderBy(x => x.OccurredAt)
                    .ToListAsync(stoppingToken);

                foreach (var outboxEvent in events)
                {
                    try
                    {
                        var eventType = Type.GetType(outboxEvent.Type);
                        if (eventType == null)
                        {
                            _logger.LogWarning("Event type {Type} not found, skipping", outboxEvent.Type);
                            continue;
                        }

                        var @event = (IEvent)JsonSerializer.Deserialize(outboxEvent.Payload, eventType)!;

                        await _eventBus.PublishAsync(@event, stoppingToken);
 
                        outboxEvent.Processed = true;
                        outboxEvent.ProcessedAt = DateTime.UtcNow;

                        await _dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Event {EventType} dispatched successfully.", outboxEvent.Type);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to dispatch event {EventType}", outboxEvent.Type);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxDispatcher main loop");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Outbox Dispatcher stopped.");
    }
}