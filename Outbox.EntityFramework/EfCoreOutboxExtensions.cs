using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Outbox.Stores;

namespace Outbox.EntityFramework;

public static class EfCoreOutboxExtensions
{
    public static IServiceCollection AddOutboxEntityFramework(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? dbContextOptions)
    {
        services.AddDbContext<EfCoreOutboxContext>(dbContextOptions);
        services.AddScoped<IOutboxStore, EfCoreOutboxStore>();
        services.AddOutbox(configuration);

        return services;
    }
}