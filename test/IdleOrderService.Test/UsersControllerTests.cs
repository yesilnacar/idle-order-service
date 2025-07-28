using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Api.Controllers;
using IdleOrderService.Application.Users;
using IdleOrderService.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class UsersControllerTests
{
    [Fact]
    public async Task RegisterUser_ShouldReturnOk_WithUserDto()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var command = new RegisterUserCommand { Email = "test@example.com", FullName = "Test User" };
        var userDto = new UserDto { Id = System.Guid.NewGuid(), Email = command.Email, FullName = command.FullName };
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);
        var controller = new UsersController(mediatorMock.Object);

        // Act
        var result = await controller.RegisterUser(command, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(command.Email, returnedUser.Email);
        Assert.Equal(command.FullName, returnedUser.FullName);
    }
} 