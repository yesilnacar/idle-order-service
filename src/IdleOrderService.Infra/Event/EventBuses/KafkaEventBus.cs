using System.Text.Json;
using Confluent.Kafka;
using IdleOrderService.Core.Event;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event.EventBuses;

public interface IEventSerializer
{
    string Serialize<TEvent>(TEvent @event);
}

public class DefaultEventSerializer : IEventSerializer
{
    public string Serialize<TEvent>(TEvent @event)
    {
        return System.Text.Json.JsonSerializer.Serialize(@event, @event?.GetType() ?? typeof(object));
    }
}

public class KafkaEventBus : IEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;
    private readonly IEventSerializer _serializer;

    public KafkaEventBus(ILogger<KafkaEventBus> logger, IProducer<string, string> producer, IEventSerializer serializer)
    {
        _logger = logger;
        _producer = producer;
        _serializer = serializer;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var topic = typeof(TEvent).AssemblyQualifiedName;
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = _serializer.Serialize(@event)
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            _logger.LogInformation("Topic: {Topic} Event: {Event} Result: {Result}", 
                topic, @event.GetType().AssemblyQualifiedName, _serializer.Serialize(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to publish event to Kafka Topic: {Topic} Event: {Event}", 
                topic, @event.GetType().AssemblyQualifiedName);
            throw;
        }
    }
}