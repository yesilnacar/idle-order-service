using IdleOrderService.Application.Users;
using IdleOrderService.Application.Users.Event;
using IdleOrderService.Core;
using IdleOrderService.Core.Event;
using IdleOrderService.Core.Mediator;
using IdleOrderService.Domain.Event;
using IdleOrderService.Infra.Event;
using IdleOrderService.Infra.Event.Consumers;
using IdleOrderService.Infra.Event.EventBuses;
using IdleOrderService.Infra.Extensions;
using IdleOrderService.Infra.Persistence;
using IdleOrderService.Infra.Pipeline;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
services.AddInfra();

// services.AddSingleton<IEventBus, InMemoryEventBus>();
services.AddSingleton<IEventBus, KafkaEventBus>();

services.AddScoped<IMediator, Mediator>();

services.AddScoped(typeof(IExecutionMiddleware<,>), typeof(LoggingMiddleware<,>));
services.AddScoped(typeof(IExecutionMiddleware<,>), typeof(MetricsMiddleware<,>));
services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
services.DecorateAllEventHandlersWithRetry()
    .DecorateAllEventHandlersWithLogging();

services.AddScoped<IRequestHandler<RegisterUserCommand, UserDto>, RegisterUserCommandHandler>();

services.AddHostedService<OutboxDispatcher>();
services.AddHostedService<KafkaEventConsumer>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();