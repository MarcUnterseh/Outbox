namespace Outbox.Exceptions;

public class OutboxException : Exception
{
    public OutboxException(string? message = null, Exception? innerException = null) : base(message, innerException)
    { }
}