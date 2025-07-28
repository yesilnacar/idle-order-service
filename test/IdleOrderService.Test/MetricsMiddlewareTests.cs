using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Infra.Pipeline;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class MetricsMiddlewareTests
{
    [Fact]
    public async Task HandleAsync_ShouldMeasureAndCallNext()
    {
        var middleware = new MetricsMiddleware<string, string>();
        var nextCalled = false;
        Task<string> Next(string ctx)
        {
            nextCalled = true;
            return Task.FromResult($"next:{ctx}");
        }
        var result = await middleware.HandleAsync("test", Next, CancellationToken.None);
        Assert.True(nextCalled);
        Assert.Equal("next:test", result);
    }
} 