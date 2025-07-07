using IdleOrderService.Core;

namespace IdleOrderService.Infra.Pipeline;

public class LoggingMiddleware<TContext, TResponse> : IExecutionMiddleware<TContext, TResponse>
{
    public async Task<TResponse> HandleAsync(TContext context, Func<TContext, Task<TResponse>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Logging] Handling {typeof(TContext).Name}");
        var response = await next(context);
        Console.WriteLine($"[Logging] Handled {typeof(TContext).Name}");
        
        return response;
    }
}