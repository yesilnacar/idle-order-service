using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Core.Mediator;
using IdleOrderService.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class MediatorTests
{
    public class DummyRequest : IRequest<string> { public string Value { get; set; } }
    public class DummyHandler : IRequestHandler<DummyRequest, string>
    {
        public Task<string> HandleAsync(DummyRequest request, CancellationToken cancellationToken) => Task.FromResult($"Handled: {request.Value}");
    }
    public class DummyMiddleware : IExecutionMiddleware<DummyRequest, string>
    {
        public bool Called { get; private set; }
        public async Task<string> HandleAsync(DummyRequest context, Func<DummyRequest, Task<string>> next, CancellationToken cancellationToken)
        {
            Called = true;
            var result = await next(context);
            return $"[MW]{result}";
        }
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenHandlerNotFound()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var mediator = new Mediator(services);
        var request = new DummyRequest { Value = "test" };
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(request));
    }

    [Fact]
    public async Task Send_ShouldCallHandler_AndMiddleware()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<DummyRequest, string>, DummyHandler>();
        services.AddScoped(typeof(IExecutionMiddleware<DummyRequest, string>), typeof(DummyMiddleware));
        var provider = services.BuildServiceProvider();
        var mediator = new Mediator(provider);
        var request = new DummyRequest { Value = "test" };
        var result = await mediator.Send(request);
        Assert.StartsWith("[MW]Handled: test", result);
    }
} 