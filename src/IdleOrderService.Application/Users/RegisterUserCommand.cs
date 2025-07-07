using IdleOrderService.Core.Mediator;

namespace IdleOrderService.Application.Users;

public class RegisterUserCommand : IRequest<UserDto>
{
    public string FullName { get; set; }
    public string Email { get; set; }
}