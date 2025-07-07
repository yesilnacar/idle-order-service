using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;

namespace IdleOrderService.Application.Users.Event;

public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    public Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Event] User registered: {@event.UserId}, Email: {@event.Email}, FullName: {@event.FullName}");
        return Task.CompletedTask;
    }
}