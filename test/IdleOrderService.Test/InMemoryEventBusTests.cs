using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Core.Event;
using IdleOrderService.Infra.Event;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class InMemoryEventBusTests
{
    public class DummyEvent : IEvent { }

    [Fact]
    public async Task PublishAsync_ShouldThrow_WhenNoHandler()
    {
        var services = new ServiceCollection();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(services.BuildServiceProvider());
        scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);
        var bus = new InMemoryEventBus(scopeFactory.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() => bus.PublishAsync(new DummyEvent(), CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_ShouldCallHandler_WhenHandlerExists()
    {
        var handlerMock = new Mock<IEventHandler<DummyEvent>>();
        handlerMock.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
        var services = new ServiceCollection();
        services.AddSingleton(typeof(IEventHandler<DummyEvent>), handlerMock.Object);
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(services.BuildServiceProvider());
        scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);
        var bus = new InMemoryEventBus(scopeFactory.Object);
        await bus.PublishAsync(new DummyEvent(), CancellationToken.None);
        handlerMock.Verify(h => h.HandleAsync(It.IsAny<DummyEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 