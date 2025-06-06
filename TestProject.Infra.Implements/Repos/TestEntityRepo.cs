using Dapper;
using Npgsql;
using System.Data.Common;
using TestProject.App.Infra.Contracts;
using TestProject.App.Infra.Contracts.Repos;
using TestProject.Core.Entities;

namespace TestProject.Infra.Implements.Repos;

internal class TestEntityRepo(DbConnection connection) : ITestEntityRepo
{
    private DbTransaction? _transaction;

    private readonly DbConnection _connection = connection;

    public async Task<int> CreateAsync(
        TestEntity entity,
        CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"
                INSERT INTO test_entities (id, sum, name)
                VALUES (@{nameof(TestEntity.Id)}, @{nameof(TestEntity.Sum)}, @{nameof(TestEntity.Name)})";

        var commandDefinition = new CommandDefinition(sqlQuery, 
            new { entity.Id, entity.Sum, entity.Name}, 
            _transaction, 
            cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(commandDefinition);
    }

    public async Task<TestEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            SELECT
                id AS {nameof(TestEntity.Id)}, 
                sum AS {nameof(TestEntity.Sum)}, 
                name AS {nameof(TestEntity.Name)}
            FROM test_entities 
            WHERE id = @{nameof(id)}";

        var commandDefinition = new CommandDefinition(sql, new { id }, _transaction, cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<TestEntity>(commandDefinition);
    }

    public async Task<int> AddSumAsync(
        int id,
        int sumToAdd,
        CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"
            UPDATE test_entities 
            SET sum = sum + @{nameof(sumToAdd)}
            WHERE id = @{nameof(id)}";

        var commandDefinition = new CommandDefinition(
            sqlQuery,
            new { id, sumToAdd },
            _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(commandDefinition);
    }

    public async Task<int> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"
            DELETE FROM test_entities 
            WHERE id = @{nameof(id)}";

        var commandDefinition = new CommandDefinition(
            sqlQuery,
            new { id },
            _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(commandDefinition);
    }

    internal void SetTransaction(DbTransaction transaction)
    {
        _transaction = transaction;
    }

    internal void UnsetTransaction()
    {
        _transaction = null;
    }

}
