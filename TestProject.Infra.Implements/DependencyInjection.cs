using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestProject.App.Infra.Contracts;
using TestProject.Infra.Implements.Options;

namespace TestProject.Infra.Implements;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgreSqlOptions>(configuration.GetSection(nameof(PostgreSqlOptions)));
        services.AddTransient<IDatabaseManager, DatabaseManager>();

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
