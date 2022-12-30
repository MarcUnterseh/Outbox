using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Outbox.Abstractions.Dtos;

namespace Outbox.EntityFramework.Configurations;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).IsRequired();
        builder.Property(message => message.Type).IsRequired();
        builder.Property(message => message.Data).IsRequired();
        builder.Property(message => message.CreatedUtc).IsRequired();
    }
}