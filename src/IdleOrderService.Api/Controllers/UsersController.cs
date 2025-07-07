using IdleOrderService.Application.Users;
using IdleOrderService.Core.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace IdleOrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(command, cancellationToken);
        return Ok(user);
    }
}