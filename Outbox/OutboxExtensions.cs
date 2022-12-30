using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Outbox.Abstractions;
using Outbox.Configuration;
using Outbox.Exceptions;

namespace Outbox;

public static class OutboxExtensions
{
    private const string OutboxConfigurationSectionName = "Outbox";
    private const string OutboxConfigurationDeleteAfterSectionName = "DeleteAfter";

    public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        var outboxConfiguration = VerifyConfiguration(configuration);

        services.AddSingleton(outboxConfiguration);
        services.AddScoped<IOutboxListener, OutboxListener>();

        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    private static OutboxConfiguration VerifyConfiguration(IConfiguration configuration)
    {
        var sectionKey = $"{OutboxConfigurationSectionName}:{OutboxConfigurationDeleteAfterSectionName}";
        var deleteAfterConfigurationSection = configuration.GetSection(sectionKey);

        if (!deleteAfterConfigurationSection.Exists())
            throw new OutboxConfigurationException($"Unable to find section '{sectionKey}' in configuration.");

        if (!bool.TryParse(deleteAfterConfigurationSection.Value, out bool deleteAfter))
            throw new OutboxConfigurationException($"Configuration value for section '{sectionKey}' should be a boolean.");

        return new OutboxConfiguration(deleteAfter);
    }
}