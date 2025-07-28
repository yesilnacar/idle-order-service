using IdleOrderService.Core.Event;

namespace IdleOrderService.Domain.Event;

public class UserRegisteredEvent : IEvent
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
}