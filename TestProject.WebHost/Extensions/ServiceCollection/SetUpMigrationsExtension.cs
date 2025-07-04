using FluentMigrator.Runner;
using TestProject.Infra.Implements.Migrations;
using TestProject.Infra.Implements.Options;

namespace TestProject.WebHost.Extensions.ServiceCollection;

public static class SetUpMigrationsExtension
{
    public static IServiceCollection SetUpMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        var postgreSqlOptions = configuration.GetRequiredSection(nameof(PostgreSqlOptions)).Get<PostgreSqlOptions>()
            ?? throw new InvalidOperationException($"Не найдена конфигурация '{nameof(PostgreSqlOptions)}' в настройках приложения.");

        services.AddFluentMigratorCore()
            .ConfigureRunner(x => x.AddPostgres()
                .WithGlobalConnectionString(postgreSqlOptions.WorkConnectionString)
                .ScanIn(typeof(CreateTestSchema).Assembly).For.Migrations());

        return services;
    }
}
