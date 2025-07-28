using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Application.Users;
using IdleOrderService.Application.Users.Event;
using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnUserDto_AndSaveEvent()
    {
        // Arrange
        var eventStoreMock = new Mock<IEventStore>();
        var handler = new RegisterUserCommandHandler(eventStoreMock.Object);
        var command = new RegisterUserCommand { Email = "test@example.com", FullName = "Test User" };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.HandleAsync(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Email, result.Email);
        Assert.Equal(command.FullName, result.FullName);
        Assert.NotEqual(Guid.Empty, result.Id);
        eventStoreMock.Verify(es => es.SaveAsync(It.Is<UserRegisteredEvent>(e =>
            e.Email == command.Email && e.FullName == command.FullName && e.UserId == result.Id), cancellationToken), Times.Once);
    }
} 