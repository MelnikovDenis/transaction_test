using FluentMigrator.Runner;
using TestProject.App.Infra.Contracts;

namespace TestProject.WebHost.Extensions.ServiceProvider;

public static class EnsureDbReadyExtension
{
    public static IServiceProvider EnsureDbReady(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbCreator = scope.ServiceProvider.GetRequiredService<IDatabaseManager>();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            dbCreator.EnsureDbCreated();
            runner.MigrateUp();
            dbCreator.EnsureDbEmpty();
        }

        return serviceProvider;
    }

}
