using Microsoft.EntityFrameworkCore;
using Outbox.Abstractions.Dtos;
using Outbox.EntityFramework.Configurations;

namespace Outbox.EntityFramework;

public sealed class EfCoreOutboxContext : DbContext
{
    public EfCoreOutboxContext(DbContextOptions<EfCoreOutboxContext> options) : base(options)
    {
        if (!Database.CanConnect())
        {
            Database.EnsureCreated();
        }
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}