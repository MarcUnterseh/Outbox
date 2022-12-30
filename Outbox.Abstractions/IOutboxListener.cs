using Outbox.Abstractions.Dtos;

namespace Outbox.Abstractions;

public interface IOutboxListener
{
    Task Commit(OutboxMessage message);
}