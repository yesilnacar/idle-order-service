using System;
using IdleOrderService.Infra.Event;
using Xunit;

namespace IdleOrderService.Test;

public class OutboxEventTests
{
    [Fact]
    public void OutboxEvent_ShouldHaveDefaultValues()
    {
        // Act
        var outboxEvent = new OutboxEvent();
        
        // Assert
        Assert.NotEqual(Guid.Empty, outboxEvent.Id);
        Assert.Equal(1, outboxEvent.Priority);
        Assert.False(outboxEvent.Processed);
        Assert.Null(outboxEvent.ProcessedAt);
        Assert.True(outboxEvent.OccurredAt > DateTime.MinValue);
        Assert.True(outboxEvent.OccurredAt <= DateTime.UtcNow);
    }
    
    [Fact]
    public void OutboxEvent_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var occurredAt = DateTime.UtcNow.AddHours(-1);
        var processedAt = DateTime.UtcNow;
        
        // Act
        var outboxEvent = new OutboxEvent
        {
            Id = id,
            Type = "TestEvent",
            Payload = "{\"test\":\"value\"}",
            OccurredAt = occurredAt,
            Processed = true,
            ProcessedAt = processedAt,
            Priority = 5
        };
        
        // Assert
        Assert.Equal(id, outboxEvent.Id);
        Assert.Equal("TestEvent", outboxEvent.Type);
        Assert.Equal("{\"test\":\"value\"}", outboxEvent.Payload);
        Assert.Equal(occurredAt, outboxEvent.OccurredAt);
        Assert.True(outboxEvent.Processed);
        Assert.Equal(processedAt, outboxEvent.ProcessedAt);
        Assert.Equal(5, outboxEvent.Priority);
    }
    
    [Fact]
    public void OutboxEvent_ShouldHandleNullProcessedAt()
    {
        // Act
        var outboxEvent = new OutboxEvent
        {
            Processed = false,
            ProcessedAt = null
        };
        
        // Assert
        Assert.False(outboxEvent.Processed);
        Assert.Null(outboxEvent.ProcessedAt);
    }
    
    [Fact]
    public void OutboxEvent_ShouldHandleEmptyPayload()
    {
        // Act
        var outboxEvent = new OutboxEvent
        {
            Payload = ""
        };
        
        // Assert
        Assert.Equal("", outboxEvent.Payload);
    }
    
    [Fact]
    public void OutboxEvent_ShouldHandleNullType()
    {
        // Act
        var outboxEvent = new OutboxEvent
        {
            Type = null!
        };
        
        // Assert
        Assert.Null(outboxEvent.Type);
    }
} 