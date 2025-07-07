using Microsoft.Extensions.DependencyInjection;

namespace IdleOrderService.Core.Mediator;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler not found for {request.GetType().Name}");

        var pipelineType = typeof(IExecutionMiddleware<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var middlewares = serviceProvider.GetServices(pipelineType)
            .Cast<dynamic>()
            .Reverse()
            .ToList();

        Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> handlerInvoker =
            (req, ct) => ((dynamic)handler).HandleAsync((dynamic)req, ct);

        Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> pipelineChain = (req, ct) => handlerInvoker(req, ct);

        foreach (var middleware in middlewares)
        {
            var next = pipelineChain;
            pipelineChain = async (req, ct) =>
                await middleware.HandleAsync((dynamic)req, new Func<object, Task<TResponse>>(ctx => next((IRequest<TResponse>)ctx, ct)), cancellationToken);
        }

        return await pipelineChain(request, cancellationToken);
    }
}