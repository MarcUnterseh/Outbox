using Outbox.Abstractions;
using Outbox.Abstractions.Dtos;

namespace Outbox.ConsoleApplication;

internal class OutboxMessageHandler : IOutboxMessageHandler
{
    public Task<bool> Handle(OutboxMessage message)
    {
        Console.WriteLine("Message handled :");
        Console.WriteLine($"Type : {message.Type}");
        Console.WriteLine($"Data : {message.Data}");
        return Task.FromResult(true);
    }
}