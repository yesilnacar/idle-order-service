using System.Diagnostics;
using IdleOrderService.Core;

namespace IdleOrderService.Infra.Pipeline;

public class MetricsMiddleware<TContext, TResponse> : IExecutionMiddleware<TContext, TResponse>
{
    public async Task<TResponse> HandleAsync(TContext context, Func<TContext, Task<TResponse>> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next(context);
        sw.Stop();
        
        Console.WriteLine($"[Metrics] {typeof(TContext).Name} took {sw.ElapsedMilliseconds} ms");
        
        return response;
    }
}