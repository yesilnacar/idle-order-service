using System.Diagnostics.CodeAnalysis;
using IdleOrderService.Core.Event;
using Microsoft.Extensions.Logging;

namespace IdleOrderService.Infra.Event;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class RetryingEventHandlerDecorator<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
{
    private readonly IEventHandler<TEvent> _inner;
    private readonly ILogger<RetryingEventHandlerDecorator<TEvent>> _logger;
    private readonly int _maxRetryCount;
    private readonly TimeSpan _retryDelay;
    
    public RetryingEventHandlerDecorator(IEventHandler<TEvent> inner,
        ILogger<RetryingEventHandlerDecorator<TEvent>> logger)
    {
        _inner = inner;
        _logger = logger;
        _maxRetryCount = 3;
        _retryDelay = TimeSpan.FromMilliseconds(500);
    }
    
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        var attempt = 0;
        Exception? lastException = null;

        for (; attempt < _maxRetryCount; attempt++)
        {
            try
            {
                await _inner.HandleAsync(@event, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning(ex, "[Retry] {Event} failed on attempt {Attempt}", typeof(TEvent).Name, attempt + 1);
                await Task.Delay(_retryDelay, cancellationToken);
            }
        }

        _logger.LogError(lastException, "[Retry] {Event} failed after {Count} attempts", typeof(TEvent).Name, _maxRetryCount);
        throw lastException!;
    }
}
