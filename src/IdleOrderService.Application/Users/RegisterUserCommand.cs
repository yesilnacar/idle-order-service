using IdleOrderService.Core.Mediator;

namespace IdleOrderService.Application.Users;

public class RegisterUserCommand : IRequest<UserDto>
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
}