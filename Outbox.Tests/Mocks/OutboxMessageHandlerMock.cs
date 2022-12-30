using Moq;
using Outbox.Abstractions;
using Outbox.Abstractions.Dtos;

namespace Outbox.Tests.Mocks;

internal class OutboxMessageHandlerMock : Mock<IOutboxMessageHandler>
{
    public OutboxMessageHandlerMock SetupHandle(OutboxMessage message, bool expectedResult)
    {
        Setup(handler => handler.Handle(message)).Returns(Task.FromResult(expectedResult));

        return this;
    }
}