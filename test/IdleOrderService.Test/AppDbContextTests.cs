using System;
using System.Linq;
using IdleOrderService.Infra.Event;
using IdleOrderService.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace IdleOrderService.Test;

public class AppDbContextTests
{
    [Fact]
    public void AppDbContext_ShouldHaveOutboxEventsDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbContextTestDb")
            .Options;
        
        // Act
        using var dbContext = new AppDbContext(options);
        
        // Assert
        Assert.NotNull(dbContext.OutboxEvents);
        Assert.IsAssignableFrom<DbSet<OutboxEvent>>(dbContext.OutboxEvents);
    }
    
    [Fact]
    public async Task AppDbContext_ShouldSaveAndRetrieveOutboxEvent()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbContextSaveTestDb")
            .Options;
        
        await using var dbContext = new AppDbContext(options);
        var outboxEvent = new OutboxEvent
        {
            Type = "TestEvent",
            Payload = "{}",
            OccurredAt = DateTime.UtcNow,
            Processed = false,
            Priority = 1
        };
        
        // Act
        dbContext.OutboxEvents.Add(outboxEvent);
        await dbContext.SaveChangesAsync();
        
        // Assert
        var retrievedEvent = await dbContext.OutboxEvents.FirstOrDefaultAsync();
        Assert.NotNull(retrievedEvent);
        Assert.Equal("TestEvent", retrievedEvent.Type);
        Assert.Equal("{}", retrievedEvent.Payload);
        Assert.False(retrievedEvent.Processed);
        Assert.Equal(1, retrievedEvent.Priority);
        Assert.NotEqual(Guid.Empty, retrievedEvent.Id);
        Assert.True(retrievedEvent.OccurredAt > DateTime.MinValue);
    }
    
    [Fact]
    public async Task AppDbContext_ShouldUpdateOutboxEvent()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbContextUpdateTestDb")
            .Options;
        
        await using var dbContext = new AppDbContext(options);
        var outboxEvent = new OutboxEvent
        {
            Type = "TestEvent",
            Payload = "{}",
            Processed = false
        };
        
        dbContext.OutboxEvents.Add(outboxEvent);
        await dbContext.SaveChangesAsync();
        
        // Act
        outboxEvent.Processed = true;
        outboxEvent.ProcessedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        
        // Assert
        var updatedEvent = await dbContext.OutboxEvents.FirstOrDefaultAsync();
        Assert.NotNull(updatedEvent);
        Assert.True(updatedEvent.Processed);
        Assert.NotNull(updatedEvent.ProcessedAt);
    }
    
    [Fact]
    public async Task AppDbContext_ShouldQueryUnprocessedEvents()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbContextQueryTestDb")
            .Options;
        
        await using var dbContext = new AppDbContext(options);
        
        var processedEvent = new OutboxEvent
        {
            Type = "ProcessedEvent",
            Payload = "{}",
            Processed = true,
            ProcessedAt = DateTime.UtcNow
        };
        
        var unprocessedEvent = new OutboxEvent
        {
            Type = "UnprocessedEvent",
            Payload = "{}",
            Processed = false
        };
        
        dbContext.OutboxEvents.AddRange(processedEvent, unprocessedEvent);
        await dbContext.SaveChangesAsync();
        
        // Act
        var unprocessedEvents = await dbContext.OutboxEvents
            .Where(x => !x.Processed)
            .ToListAsync();
        
        // Assert
        Assert.Single(unprocessedEvents);
        Assert.Equal("UnprocessedEvent", unprocessedEvents[0].Type);
    }
} 