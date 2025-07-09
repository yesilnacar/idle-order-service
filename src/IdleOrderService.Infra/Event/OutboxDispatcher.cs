using System.Text.Json;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event;

public class OutboxDispatcher : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxDispatcher> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public OutboxDispatcher(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxDispatcher> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Dispatcher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var events = await dbContext.OutboxEvents
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

                        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                        await eventBus.PublishAsync(@event, stoppingToken);

                        outboxEvent.Processed = true;
                        outboxEvent.ProcessedAt = DateTime.UtcNow;

                        await dbContext.SaveChangesAsync(stoppingToken);

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