using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Outbox.Abstractions;
using Outbox.Exceptions;
using Outbox.Stores;
using Outbox.Tests.Mocks;
using Xunit;

namespace Outbox.Tests;

public class OutboxExtensionsTests
{
    private readonly ConfigurationMock _configurationMock;
    private readonly IServiceCollection _serviceCollection;

    public OutboxExtensionsTests()
    {
        _serviceCollection = new ServiceCollection();
        _configurationMock = new ConfigurationMock();

        _serviceCollection.AddScoped<IOutboxStore>(_ => new OutboxStoreMock().Object);
    }

    [Fact]
    public void AddOutbox_ShouldThrowException_WhenDeleteAfterIsNotConfigured()
    {
        Action addOutbox = () => _serviceCollection.AddOutbox(_configurationMock.Object);

        addOutbox.Should().Throw<OutboxConfigurationException>();
    }

    [Fact]
    public void AddOutbox_ShouldThrowException_WhenDeleteAfterIsNotABoolean()
    {
        _configurationMock.SetupGetSection("Outbox:DeleteAfter", "NotABoolean");

        Action addOutbox = () => _serviceCollection.AddOutbox(_configurationMock.Object);

        addOutbox.Should().Throw<OutboxConfigurationException>();
    }

    [Fact]
    public void AddOutbox_ShouldAddOutboxListenerToServiceCollection()
    {
        _configurationMock.SetupGetSection("Outbox:DeleteAfter", "true");

        _serviceCollection.AddOutbox(_configurationMock.Object);
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        serviceProvider.GetService<IOutboxListener>().Should().NotBeNull().And.BeAssignableTo<OutboxListener>();
    }
}