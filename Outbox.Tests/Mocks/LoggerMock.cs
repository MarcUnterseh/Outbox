using Microsoft.Extensions.Logging;
using Moq;

namespace Outbox.Tests.Mocks;

internal class LoggerMock<T> : Mock<ILogger<T>>
{
    public LoggerMock<T> VerifyError(Exception exception, Times expectedTimes)
    {
        Verify(logger =>
            logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), expectedTimes);

        return this;
    }
}