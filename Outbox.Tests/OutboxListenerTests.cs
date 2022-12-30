using Outbox.Abstractions.Dtos;
using Outbox.Tests.Mocks;
using Xunit;

namespace Outbox.Tests;

public class OutboxListenerTests
{
    private readonly OutboxStoreMock _outboxStoreMock;
    private readonly OutboxListener _outboxListener;

    public OutboxListenerTests()
    {
        _outboxStoreMock = new OutboxStoreMock();
        _outboxListener = new OutboxListener(_outboxStoreMock.Object);
    }

    [Fact]
    public async void Commit_ShouldAddMessageToStore()
    {
        var message = new OutboxMessage("Type", "Data");

        await _outboxListener.Commit(message);

        _outboxStoreMock.Verify(store => store.Add(message));
    }
}