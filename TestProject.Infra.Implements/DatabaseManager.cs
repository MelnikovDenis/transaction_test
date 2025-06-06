using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using TestProject.App.Infra.Contracts;
using TestProject.Infra.Implements.Options;

namespace TestProject.Infra.Implements;

internal class DatabaseManager(IOptions<PostgreSqlOptions> options, ILogger<DatabaseManager> logger) : IDatabaseManager
{
    private readonly PostgreSqlOptions _options = options.Value;

    private readonly ILogger<DatabaseManager> _logger = logger;

    public void EnsureDbCreated()
    {
        var checkSql = $@"
            SELECT 1 FROM pg_database WHERE datname = @{nameof(_options.DatabaseName)}
        ";

        var createSql = $@"
            CREATE DATABASE ""{_options.DatabaseName}""
        ";

        using (var connection = new NpgsqlConnection(_options.InitConnectionString))
        {
            var dbExists = connection.ExecuteScalar<bool>(checkSql, new { _options.DatabaseName });

            if (!dbExists)
            {
                _logger.LogInformation("Первичное создание БД...");

                connection.Execute(createSql);

                _logger.LogInformation("Первичное создание БД. Успешно.");
            }
        }
    }

    public void EnsureDbEmpty()
    {
        var truncateSql = $@"
            TRUNCATE TABLE test_entities
        ";

        using (var connection = new NpgsqlConnection(_options.TruncateConnectionString))
        {
            _logger.LogInformation("Очистка таблиц...");

            connection.Execute(truncateSql);

            _logger.LogInformation("Очистка таблиц. Успешно.");
        }
    }
}
