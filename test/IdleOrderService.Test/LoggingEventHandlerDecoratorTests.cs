using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Event;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class LoggingEventHandlerDecoratorTests
{
    public class DummyEvent : IEvent { }

    [Fact]
    public async Task HandleAsync_ShouldLogInfo_WhenSuccess()
    {
        var innerMock = new Mock<IEventHandler<DummyEvent>>();
        innerMock.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var loggerMock = new Mock<ILogger<LoggingEventHandlerDecorator<DummyEvent>>>();
        var decorator = new LoggingEventHandlerDecorator<DummyEvent>(innerMock.Object, loggerMock.Object);
        await decorator.HandleAsync(new DummyEvent(), CancellationToken.None);
        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task HandleAsync_ShouldLogError_AndThrow_WhenException()
    {
        var innerMock = new Mock<IEventHandler<DummyEvent>>();
        innerMock.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));
        var loggerMock = new Mock<ILogger<LoggingEventHandlerDecorator<DummyEvent>>>();
        var decorator = new LoggingEventHandlerDecorator<DummyEvent>(innerMock.Object, loggerMock.Object);
        await Assert.ThrowsAsync<Exception>(() => decorator.HandleAsync(new DummyEvent(), CancellationToken.None));
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }
} 