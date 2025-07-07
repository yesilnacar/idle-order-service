using IdleOrderService.Core.Event;
using IdleOrderService.Core.Mediator;
using IdleOrderService.Domain.Event;

namespace IdleOrderService.Application.Users;

public class RegisterUserCommandHandler(IEventBus eventBus,
    IEventStore eventStore) : IRequestHandler<RegisterUserCommand, UserDto>
{
    public async Task<UserDto> HandleAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName
        };

        Console.WriteLine($"[DB] User registered: {user.Id} {user.Email}, FullName: {user.FullName}");

        var userRegisteredEvent = new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };

        //await eventBus.PublishAsync(userRegisteredEvent, cancellationToken);
        await eventStore.SaveAsync(userRegisteredEvent, cancellationToken);
        
        return user;
    }
}