namespace IdleOrderService.Core;

public interface IExecutionMiddleware<TContext, TResponse>
{
    Task<TResponse> HandleAsync(TContext context, Func<TContext, Task<TResponse>> next, CancellationToken cancellationToken);
}