namespace Outbox.Abstractions.Dtos;

public class OutboxMessage
{
    public OutboxMessage(string type = "", string data = "")
    {
        Type = type;
        Data = data;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedUtc { get; } = DateTime.UtcNow;
    public string Type { get; } 
    public string Data { get; } 
    public DateTime? Processed { get; set; }
}