using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using TestProject.App.Infra.Contracts;
using TestProject.Core.Entities;
using TestProject.Infra.Implements.Options;

namespace TestProject.Infra.Implements;

internal class DatabaseManager(IOptions<PostgreSqlOptions> postgreSqlOptions, 
    IOptions<SeedingOptions> seedingOptions,
    ILogger<DatabaseManager> logger,
    IServiceProvider serviceProvider) : IDatabaseManager
{
    private readonly PostgreSqlOptions _postgreSqlOptions = postgreSqlOptions.Value;

    private readonly SeedingOptions _seedingOptions = seedingOptions.Value;

    private readonly ILogger<DatabaseManager> _logger = logger;

    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void EnsureDbCreated()
    {
        var checkSql = $@"
            SELECT 1 FROM pg_database WHERE datname = @{nameof(_postgreSqlOptions.DatabaseName)}
        ";

        var createSql = $@"
            CREATE DATABASE ""{_postgreSqlOptions.DatabaseName}""
        ";

        using var connection = new NpgsqlConnection(_postgreSqlOptions.InitConnectionString);

        var dbExists = connection.ExecuteScalar<bool>(checkSql, new { _postgreSqlOptions.DatabaseName });

        if (!dbExists)
        {
            _logger.LogInformation("Первичное создание БД...");

            connection.Execute(createSql);

            _logger.LogInformation("Первичное создание БД. Успешно.");
        }
    }

    public void EnsureDbEmpty()
    {
        const string truncateSql = @"
            TRUNCATE TABLE test_sub_entities, test_entities CASCADE
        ";

        using var connection = new NpgsqlConnection(_postgreSqlOptions.TruncateConnectionString);

        _logger.LogInformation("Очистка таблиц...");

        connection.Open();
        connection.Execute(truncateSql);

        _logger.LogInformation("Очистка таблиц. Успешно.");
    }

    public void SeedDb()
    {
        using var scope = _serviceProvider.CreateScope();

        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _logger.LogInformation("Инициализация бд базовыми значениями...");

        uow.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            for (var i = 0; i < _seedingOptions.TestEntityCount; ++i)
            {
                var testEntity = SeedGenerator.GetRandomTestEntity();

                testEntity.Id = uow.TestEntityRepo.Create(testEntity);

                var subTestEntitiesCount = SeedGenerator.GetRandomSubTestEntityCount(_seedingOptions.MaxSubTestEntityCount);

                for (var j = 0; j < subTestEntitiesCount; ++j)
                {
                    var subTestEntity = SeedGenerator.GetRandomSubTestEntity(testEntity.Id);

                    subTestEntity.Id = uow.SubTestEntityRepo.Create(subTestEntity);
                }
            }

            uow.CommitTransaction();

            _logger.LogInformation("Инициализация бд базовыми значениями. Успешно.");
        }
        catch (Exception ex)
        {
            uow.RollbackTransaction();

            _logger.LogError(ex, "Ошибка инициализации бд базовыми значениями");
        }

    }
}
