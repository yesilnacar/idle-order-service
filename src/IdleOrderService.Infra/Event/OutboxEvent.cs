namespace IdleOrderService.Infra.Event;

public class OutboxEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;
    public DateTime? ProcessedAt { get; set; }
    public int Priority { get; set; } = 1;
}