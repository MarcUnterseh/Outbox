using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Outbox.Abstractions;
using Outbox.Abstractions.Dtos;
using Outbox.ConsoleApplication;
using Outbox.EntityFramework;

Console.WriteLine("Console application to test outbox");

var host = new HostBuilder()
    .ConfigureServices(ConfigureServices)
    .ConfigureLogging(configLogging =>
        {
            configLogging.AddConsole();
        })
    .Build();

await host.Services.GetRequiredService<IOutboxListener>().Commit(new OutboxMessage("TestType", "TestData"));
await host.RunAsync();

Console.ReadKey();

static void ConfigureServices(IServiceCollection serviceCollection)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appSettings.json", false)
        .Build();

    serviceCollection.AddSingleton(configuration);
    serviceCollection.AddLogging();
    serviceCollection.AddSingleton<IOutboxMessageHandler, OutboxMessageHandler>();
    serviceCollection.AddOutboxEntityFramework(configuration, builder => builder.UseSqlServer(configuration.GetConnectionString("OutboxConnection")));
}
