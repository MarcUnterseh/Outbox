using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Outbox.Abstractions;
using Outbox.Configuration;
using Outbox.Stores;

namespace Outbox;

internal class OutboxProcessor : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOutboxMessageHandler _outboxMessageHandler;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly bool _deleteAfter;
    private Timer? _timer;

    public OutboxProcessor(
        OutboxConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        IOutboxMessageHandler outboxMessageHandler,
        ILogger<OutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _outboxMessageHandler = outboxMessageHandler;
        _logger = logger;
        _deleteAfter = configuration.DeleteAfter;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SendOutboxMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void SendOutboxMessages(object? state)
    {
        _ = Process();
    }

    public async Task Process()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
        var messageIds = await store.GetUnprocessedMessageIds();
        var publishedMessageIds = new List<Guid>();
        try
        {
            foreach (var messageId in messageIds)
            {
                var message = await store.GetMessage(messageId);

                if (message.Processed.HasValue)
                    continue;

                if (await _outboxMessageHandler.Handle(message))
                {
                    await store.SetMessageToProcessed(message.Id);
                    publishedMessageIds.Add(message.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing message from outbox");
        }
        finally
        {
            if (_deleteAfter)
            {
                await store.Delete(publishedMessageIds);
            }
        }
    }
}