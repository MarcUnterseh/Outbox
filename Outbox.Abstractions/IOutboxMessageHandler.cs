using Outbox.Abstractions.Dtos;

namespace Outbox.Abstractions;

public interface IOutboxMessageHandler
{
    Task<bool> Handle(OutboxMessage message);
}