using IdleOrderService.Infra.Event;
using Microsoft.EntityFrameworkCore;

namespace IdleOrderService.Infra.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxEvent>(builder =>
        {
            builder.ToTable("outbox_events");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Payload).IsRequired();
            builder.Property(x => x.OccurredAt).IsRequired();
            builder.Property(x => x.Processed).HasDefaultValue(false);
            builder.Property(x => x.ProcessedAt).IsRequired(false);
        });
    }
}