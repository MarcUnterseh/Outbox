using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Outbox.Tests.Boundaries;

public class ConfigurationTests
{
    private readonly IConfiguration _configuration;

    private const string FakeKey1 = "FakeKey1";
    private const string FakeKey2 = "FakeKey2";
    private const string FakeKey3 = "FakeKey3";
    private const string FakeSection1 = "FakeSection1";
    private const string FakeValue1 = "FakeValue1";
    private const string FakeValue2 = "FakeValue2";
    private const string FakeValue3 = "FakeValue3";

    public ConfigurationTests()
    {
        IEnumerable<KeyValuePair<string, string?>> myConfiguration = new Dictionary<string, string?>
        {
            { FakeKey1, FakeValue1 },
            { $"{FakeSection1}:{FakeKey2}", FakeValue2 },
            { $"{FakeSection1}:{FakeKey3}", FakeValue3 }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    [Fact]
    public void GetSection_ShouldReturnSection_WhenSectionExist()
    {
        var section = _configuration.GetSection(FakeKey1);

        section.Should().NotBeNull();
    }

    [Fact]
    public void GetSection_ShouldReturnSection_WhenSectionDoesNotExist()
    {
        var section = _configuration.GetSection("NotExistingSection");

        section.Should().NotBeNull();
    }

    [Fact]
    public void Exists_ShouldReturnTrue_WhenSectionExist()
    {
        var section = _configuration.GetSection(FakeKey1);

        var result = section.Exists();

        result.Should().BeTrue();
    }

    [Fact]
    public void Exists_ShouldReturnFalse_WhenSectionDoesNotExist()
    {
        var section = _configuration.GetSection("NotExistingSection");

        var result = section.Exists();

        result.Should().BeFalse();
    }

    [Fact]
    public void Exists_ShouldReturnTrue_WhenSectionIsAnObject()
    {
        var section = _configuration.GetSection(FakeKey1);

        var result = section.Exists();

        result.Should().BeTrue();
    }

    [Fact]
    public void Value_ShouldReturnValue_WhenSectionExist()
    {
        var section = _configuration.GetSection(FakeKey1);

        var result = section.Value;

        result.Should().BeEquivalentTo(FakeValue1);
    }

    [Fact]
    public void Value_ShouldReturnNull_WhenSectionDoesNotExist()
    {
        var section = _configuration.GetSection("NotExistingSection");

        var result = section.Value;

        result.Should().BeNull();
    }

    [Fact]
    public void Value_ShouldReturnNull_WhenSectionIsAnObject()
    {
        var section = _configuration.GetSection(FakeSection1);

        var result = section.Value;

        result.Should().BeNull();
    }
    
    [Fact]
    public void Value_OnSubSection_ShouldReturnValue_WhenSectionExist()
    {
        var section = _configuration.GetSection($"{FakeSection1}:{FakeKey2}");

        var result = section.Value;

        result.Should().BeEquivalentTo(FakeValue2);
    }
}