using System.Text.Json;
using Confluent.Kafka;
using IdleOrderService.Core.Event;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event.EventBuses;

public class KafkaEventBus : IEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;

    public KafkaEventBus(ILogger<KafkaEventBus> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
        };
        _producer = new ProducerBuilder<string, string>(config)
            .Build();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var topic = typeof(TEvent).AssemblyQualifiedName;
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            _logger.LogInformation("Topic: {Topic} Event: {Event} Result: {Result}", 
                topic, @event.GetType().AssemblyQualifiedName, JsonSerializer.Serialize(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to publish event to Kafka Topic: {Topic} Event: {Event}", 
                topic, @event.GetType().AssemblyQualifiedName);
            throw;
        }
    }
}