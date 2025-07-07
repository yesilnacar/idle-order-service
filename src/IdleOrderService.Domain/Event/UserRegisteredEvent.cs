using IdleOrderService.Core.Event;

namespace IdleOrderService.Domain.Event;

public class UserRegisteredEvent : IEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}