using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace IdleOrderService.Core.Mediator
{
    public class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        private static readonly ConcurrentDictionary<(Type req, Type res), Type> _handlerTypeCache = new();

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType  = request.GetType();
            var responseType = typeof(TResponse);

            var handlerType = _handlerTypeCache.GetOrAdd(
                (requestType, responseType),
                key => typeof(IRequestHandler<,>).MakeGenericType(key.req, key.res)
            );
            var handler = serviceProvider.GetService(handlerType)
                              ?? throw new InvalidOperationException($"Handler not found: {requestType.Name}");

            var pipelineType = typeof(IExecutionMiddleware<,>)
                .MakeGenericType(requestType, responseType);

            var middlewares = serviceProvider.GetServices(pipelineType)
                .Cast<dynamic>()
                .Reverse()
                .ToList();

            Func<IRequest<TResponse>, CancellationToken, Task<TResponse>> pipeline = (req, ct) 
                  => ((dynamic)handler).HandleAsync((dynamic)req, ct);

            foreach (var mw in middlewares)
            {
                var next = pipeline;
                pipeline = (req, ct) => mw.HandleAsync(
                        (dynamic)req,
                        new Func<object, Task<TResponse>>(ctx => next((IRequest<TResponse>)ctx, ct)),
                        ct
                    );
            }

            return await pipeline(request, cancellationToken);
        }
    }
}