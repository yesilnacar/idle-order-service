using System.Text.Json;
using Confluent.Kafka;
using IdleOrderService.Core.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event.Consumers;

public class KafkaEventConsumer(IServiceProvider serviceProvider, ILogger<KafkaEventConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = "idle-order-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("UserRegisteredEvent");

        logger.LogInformation("Kafka consumer started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                var type = Type.GetType("IdleOrderService.Domain.Event.UserRegisteredEvent, IdleOrderService.Domain");
                if (type == null)
                {
                    logger.LogWarning("Unknown event type received");
                    continue;
                }

                var @event = (IEvent)JsonSerializer.Deserialize(result.Message.Value, type)!;

                var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                var method = handlerType.GetMethod("HandleAsync")!;
                
                await (Task)method.Invoke(handler, [@event, stoppingToken])!;
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Kafka consume error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "General error in Kafka consumer");
            }
        }
    }
}
