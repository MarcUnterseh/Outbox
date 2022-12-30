using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Outbox.Abstractions.Dtos;
using Outbox.Configuration;
using Outbox.Tests.Mocks;
using Xunit;

namespace Outbox.Tests;

public class OutboxProcessorTests
{
    private const int Delay = 100;
    private readonly OutboxConfiguration _outboxConfiguration;
    private readonly OutboxMessageHandlerMock _outBoxMessageHandlerMock;
    private readonly LoggerMock<OutboxProcessor> _loggerMock;
    private readonly OutboxStoreMock _outboxStoreMock;
    private readonly OutboxProcessor _outboxProcessor;

    public OutboxProcessorTests()
    {
        _outboxConfiguration = new OutboxConfiguration(true);
        _outboxStoreMock = new OutboxStoreMock();
        _outBoxMessageHandlerMock = new OutboxMessageHandlerMock();
        _loggerMock = new LoggerMock<OutboxProcessor>();
        var serviceScopeFactoryMock = CreateServiceScopeFactoryMock();

        _outboxProcessor = new OutboxProcessor(
            _outboxConfiguration,
            serviceScopeFactoryMock.Object,
            _outBoxMessageHandlerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void OutboxProcessor_ShouldNotCheckForNewMessages_BeforeStarted()
    {
        _outboxStoreMock.Verify(store => store.GetUnprocessedMessageIds(), Times.Never);
    }

    [Fact]
    public async void OutboxProcessor_ShouldCheckForNewMessages_WhenStarted()
    {
        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outboxStoreMock.Verify(store => store.GetUnprocessedMessageIds(), Times.AtLeastOnce);
    }

    [Fact]
    public async void OutboxProcessor_ShouldStopCheckingForNewMessages_WhenStoped()
    {
        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outboxStoreMock.Verify(store => store.GetUnprocessedMessageIds(), Times.AtLeastOnce);
        _outboxStoreMock.Invocations.Clear();

        await _outboxProcessor.StopAsync(CancellationToken.None);
        _outboxStoreMock.Verify(store => store.GetUnprocessedMessageIds(), Times.Never);
    }

    [Fact]
    public async void OutboxProcessor_ShouldReadMessages_WhenStoreReturnsUnprocessedMessagesIds()
    {
        var messageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outboxStoreMock.Verify(store => store.GetMessage(messageIds[0]), Times.Once);
        _outboxStoreMock.Verify(store => store.GetMessage(messageIds[1]), Times.Once);
    }

    [Fact]
    public async void OutboxProcessor_ShouldIgnoreMessage_WhenMessageHasAlreadyBeenProcessed()
    {
        Guid id = Guid.NewGuid();
        var messageIds = new List<Guid> { id };
        var message = new OutboxMessage(string.Empty, string.Empty) { Processed = DateTime.Now.AddDays(-1) };

        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);
        _outboxStoreMock.SetupGetMessage(id, message);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outBoxMessageHandlerMock.Verify(handler => handler.Handle(message), Times.Never);
    }

    [Fact]
    public async void OutboxProcessor_ShouldHandleMessage_WhenStoreReturnNotProcessedMessage()
    {
        Guid id = Guid.NewGuid();
        var messageIds = new List<Guid> { id };
        var message = new OutboxMessage(string.Empty, string.Empty);

        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);
        _outboxStoreMock.SetupGetMessage(id, message);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outBoxMessageHandlerMock.Verify(handler => handler.Handle(message), Times.Once);
    }

    [Fact]
    public async void OutboxProcessor_ShouldSetMessageAsProcessed_WhenHandlerSuccess()
    {
        Guid id = Guid.NewGuid();
        var messageIds = new List<Guid> { id };
        var message = new OutboxMessage(string.Empty, string.Empty) { Id = id };

        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);
        _outboxStoreMock.SetupGetMessage(id, message);
        _outBoxMessageHandlerMock.SetupHandle(message, true);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outboxStoreMock.Verify(store => store.SetMessageToProcessed(id), Times.Once);
    }

    [Fact]
    public async void OutboxProcessor_ShouldDeleteProcessedMessages_WhenDeleteAfterIsTrue()
    {
        Guid id = Guid.NewGuid();
        var messageIds = new List<Guid> { id };
        var message = new OutboxMessage(string.Empty, string.Empty) { Id = id };

        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);
        _outboxStoreMock.SetupGetMessage(id, message);
        _outBoxMessageHandlerMock.SetupHandle(message, true);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _outboxStoreMock.Verify(store => store.Delete(messageIds), Times.Once);
    }

    [Fact]
    public async void OutboxProcessor_ShouldLog_WhenExceptionHappend()
    {
        Guid id = Guid.NewGuid();
        var messageIds = new List<Guid> { id };
        var exception = new Exception("Test exception");

        _outboxStoreMock.SetupGetUnprocessedMessageIds(messageIds);
        _outboxStoreMock.SetupGetMessage(id, exception);

        await _outboxProcessor.StartAsync(CancellationToken.None);
        await Task.Delay(Delay);

        _loggerMock.VerifyError(exception, Times.Once());
    }


    private Mock<IServiceScopeFactory> CreateServiceScopeFactoryMock()
    {
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _outboxStoreMock.Object);
        serviceScopeMock.SetupGet(service => service.ServiceProvider).Returns(serviceCollection.BuildServiceProvider());
        serviceScopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(serviceScopeMock.Object);
        return serviceScopeFactoryMock;
    }
}