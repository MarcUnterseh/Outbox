namespace Outbox.Exceptions;

public class OutboxConfigurationException : OutboxException
{
    public OutboxConfigurationException(string? message = null, Exception? innerException = null) : base(message, innerException)
    { }
}