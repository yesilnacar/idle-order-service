using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Event.EventBuses;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IdleOrderService.Domain.Event;

namespace IdleOrderService.Test;

public class KafkaEventBusTests
{
    public class DummyEvent : IEvent { public string Name { get; set; } = "test"; }

    [Fact]
    public async Task PublishAsync_ShouldLogAndPublish()
    {
        var loggerMock = new Mock<ILogger<KafkaEventBus>>();
        var producerMock = new Mock<IProducer<string, string>>();
        var serializerMock = new Mock<IEventSerializer>();
        serializerMock.Setup(s => s.Serialize(It.IsAny<object>())).Returns("serialized");
        producerMock.Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResult<string, string>());
        var bus = new KafkaEventBus(loggerMock.Object, producerMock.Object, serializerMock.Object);
        var evt = new UserRegisteredEvent { UserId = Guid.NewGuid(), Email = "test@example.com", FullName = "Test User" };
        await bus.PublishAsync(evt, CancellationToken.None);
        producerMock.Verify(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_ShouldLogErrorAndThrow_OnException()
    {
        var loggerMock = new Mock<ILogger<KafkaEventBus>>();
        var producerMock = new Mock<IProducer<string, string>>();
        var serializerMock = new Mock<IEventSerializer>();
        serializerMock.Setup(s => s.Serialize(It.IsAny<object>())).Returns("serialized");
        producerMock.Setup(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Kafka error"));
        var bus = new KafkaEventBus(loggerMock.Object, producerMock.Object, serializerMock.Object);
        var evt = new UserRegisteredEvent { UserId = Guid.NewGuid(), Email = "test@example.com", FullName = "Test User" };
        await Assert.ThrowsAsync<Exception>(() => bus.PublishAsync(evt, CancellationToken.None));
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
    }
} 