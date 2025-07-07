namespace IdleOrderService.Core;

public class ExecutionPipeline<TContext, TResponse>(IEnumerable<IExecutionMiddleware<TContext, TResponse>> middlewares)
{
    private readonly IList<IExecutionMiddleware<TContext, TResponse>> _middlewares = new List<IExecutionMiddleware<TContext, TResponse>>(middlewares);

    public Task<TResponse> ExecuteAsync(TContext context, Func<TContext, Task<TResponse>> finalHandler, CancellationToken cancellationToken = default)
    {
        var next = finalHandler;

        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var currentMiddleware = _middlewares[i];
            var nextCopy = next;
            next = (ctx) => currentMiddleware.HandleAsync(ctx, nextCopy, cancellationToken);
        }

        return next(context);
    }
}