using Microsoft.Extensions.Configuration;
using Moq;

namespace Outbox.Tests.Mocks;

internal class ConfigurationMock : Mock<IConfiguration>
{
    public ConfigurationMock SetupGetSection(string key, string? expectedValue)
    {
        var resultSectionMock = new ConfigurationSectionMock().SetupGetValue(expectedValue);

        Setup(c => c.GetSection(key)).Returns(resultSectionMock.Object);
        
        return this;
    }
}


internal class ConfigurationSectionMock : Mock<IConfigurationSection>
{
    public ConfigurationSectionMock SetupGetValue(string? expectedValue)
    {
        SetupGet(c => c.Value).Returns(expectedValue);

        return this;
    }
}