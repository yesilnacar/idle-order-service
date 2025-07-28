using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;
using IdleOrderService.Infra.Event;
using IdleOrderService.Infra.Extensions;
using IdleOrderService.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdleOrderService.Test;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfra_ShouldRegisterEfEventStore()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => 
            options.UseInMemoryDatabase("TestDb"));
        
        // Act
        services.AddInfra();
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var eventStore = serviceProvider.GetService<IEventStore>();
        Assert.NotNull(eventStore);
        Assert.IsType<EfEventStore>(eventStore);
    }
} 