using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Event;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class RetryingEventHandlerDecoratorTests
{
    public class DummyEvent : IEvent { }

    [Fact]
    public async Task HandleAsync_ShouldSucceed_WithoutRetry()
    {
        var innerMock = new Mock<IEventHandler<DummyEvent>>();
        innerMock.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var loggerMock = new Mock<ILogger<RetryingEventHandlerDecorator<DummyEvent>>>();
        var decorator = new RetryingEventHandlerDecorator<DummyEvent>(innerMock.Object, loggerMock.Object);
        await decorator.HandleAsync(new DummyEvent(), CancellationToken.None);
        innerMock.Verify(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldRetry_AndLogError_WhenException()
    {
        var innerMock = new Mock<IEventHandler<DummyEvent>>();
        innerMock.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));
        var loggerMock = new Mock<ILogger<RetryingEventHandlerDecorator<DummyEvent>>>();
        var decorator = new RetryingEventHandlerDecorator<DummyEvent>(innerMock.Object, loggerMock.Object);
        await Assert.ThrowsAsync<Exception>(() => decorator.HandleAsync(new DummyEvent(), CancellationToken.None));
        innerMock.Verify(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(3));
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }
} 