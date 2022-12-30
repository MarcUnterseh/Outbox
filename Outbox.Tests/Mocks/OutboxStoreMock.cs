using Moq;
using Outbox.Abstractions.Dtos;
using Outbox.Stores;

namespace Outbox.Tests.Mocks;

internal class OutboxStoreMock : Mock<IOutboxStore>
{
    public OutboxStoreMock()
    {
        SetupGetMessage(new OutboxMessage(string.Empty, string.Empty));
    }

    public OutboxStoreMock SetupGetUnprocessedMessageIds(IEnumerable<Guid> expectedResult)
    {
        Setup(store => store.GetUnprocessedMessageIds()).Returns(Task.FromResult(expectedResult));

        return this;
    }
    public OutboxStoreMock SetupGetMessage(OutboxMessage expectedResult)
    {
        Setup(store => store.GetMessage(It.IsAny<Guid>())).Returns(Task.FromResult(expectedResult));

        return this;
    }
    public OutboxStoreMock SetupGetMessage(Guid id, OutboxMessage expectedResult)
    {
        Setup(store => store.GetMessage(id)).Returns(Task.FromResult(expectedResult));

        return this;
    }
    public OutboxStoreMock SetupGetMessage(Guid id, Exception expectedException)
    {
        Setup(store => store.GetMessage(id)).ThrowsAsync(expectedException);

        return this;
    }
}