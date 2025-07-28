using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Application.Users.Event;
using IdleOrderService.Domain.Event;
using Xunit;

namespace IdleOrderService.Test;

public class UserRegisteredEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldComplete_WhenEventIsHandled()
    {
        // Arrange
        var handler = new UserRegisteredEventHandler();
        var @event = new UserRegisteredEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User"
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await handler.HandleAsync(@event, cancellationToken);
    }
} 