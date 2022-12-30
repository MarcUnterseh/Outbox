using FluentAssertions;
using Outbox.Abstractions.Dtos;
using Xunit;

namespace Outbox.Tests;

public class OutboxMessageTests
{
    [Fact]
    public void Constructor_ShouldSetType()
    {
        const string expectedType = "TypeForTests";

        var message = new OutboxMessage(expectedType, string.Empty);

        message.Type.Should().Be(expectedType);
    }

    [Fact]
    public void Constructor_ShouldSetData()
    {
        const string expectedData = "DataForTests";

        var message = new OutboxMessage(string.Empty, expectedData);

        message.Data.Should().Be(expectedData);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedUtc()
    {
        var message = new OutboxMessage(string.Empty, string.Empty);

        message.CreatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}