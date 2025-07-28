using System;
using System.Threading;
using System.Threading.Tasks;
using IdleOrderService.Core.Event;
using IdleOrderService.Domain.Event;
using IdleOrderService.Infra.Event;
using IdleOrderService.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace IdleOrderService.Test;

public class EfEventStoreTests
{
    [Fact]
    public async Task SaveAsync_ShouldAddOutboxEvent_AndSaveChanges()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "EfEventStoreTestDb")
            .Options;
        await using var dbContext = new AppDbContext(options);
        var store = new EfEventStore(dbContext);
        var evt = new UserRegisteredEvent { UserId = Guid.NewGuid(), Email = "test@example.com", FullName = "Test User" };
        await store.SaveAsync(evt, CancellationToken.None);
        var outboxEvent = await dbContext.OutboxEvents.FirstOrDefaultAsync();
        Assert.NotNull(outboxEvent);
        Assert.Contains("UserRegisteredEvent", outboxEvent.Type);
        Assert.Contains("test@example.com", outboxEvent.Payload);
    }
} 