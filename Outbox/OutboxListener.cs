using Outbox.Abstractions;
using Outbox.Abstractions.Dtos;
using Outbox.Stores;

namespace Outbox;

public class OutboxListener : IOutboxListener
{
    private readonly IOutboxStore _store;

    public OutboxListener(IOutboxStore store)
    {
        _store = store;
    }

    public virtual async Task Commit(OutboxMessage message)
    {
        await _store.Add(message);
    }
}