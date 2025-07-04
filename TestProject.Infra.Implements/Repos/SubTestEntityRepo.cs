using Dapper;
using System.Data.Common;
using TestProject.App.Infra.Contracts.Repos;
using TestProject.Core.Entities;

namespace TestProject.Infra.Implements.Repos;

internal class SubTestEntityRepo(DbConnection connection) : ISubTestEntityRepo
{
    private DbTransaction? _transaction;

    private readonly DbConnection _connection = connection;

    #region async implements

    public async Task<int> CreateAsync(SubTestEntity entity, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            INSERT INTO test_sub_entities (name, test_entity_id)
            VALUES (@{nameof(SubTestEntity.Name)}, @{nameof(SubTestEntity.TestEntityId)})
            RETURNING id AS {nameof(SubTestEntity.Id)};";

        var command = new CommandDefinition(sql, entity, _transaction, cancellationToken: cancellationToken);
        var generatedId = await _connection.ExecuteScalarAsync<int>(command);

        return generatedId;
    }


    public async Task<SubTestEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            SELECT id AS {nameof(SubTestEntity.Id)},
                   name AS {nameof(SubTestEntity.Name)},
                   test_entity_id AS {nameof(SubTestEntity.TestEntityId)}
            FROM test_sub_entities
            WHERE id = @{nameof(id)}";

        var command = new CommandDefinition(sql, new { id }, _transaction, cancellationToken: cancellationToken);
        return await _connection.QueryFirstOrDefaultAsync<SubTestEntity>(command);
    }

    public async Task<int> UpdateNameAsync(int id, string newName, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
            UPDATE test_sub_entities
            SET name = @{nameof(newName)}
            WHERE id = @{nameof(id)}";

        var command = new CommandDefinition(sql, new { id, newName }, _transaction, cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = @"DELETE FROM test_sub_entities WHERE id = @id";

        var command = new CommandDefinition(sql, new { id }, _transaction, cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(command);
    }

    #endregion

    #region sync implements

    public int Create(SubTestEntity entity)
    {
        const string sql = $@"
            INSERT INTO test_sub_entities (name, test_entity_id)
            VALUES (@{nameof(SubTestEntity.Name)}, @{nameof(SubTestEntity.TestEntityId)})
            RETURNING id;";

        var generatedId = _connection.ExecuteScalar<int>(sql, entity, _transaction);

        return generatedId;
    }

    public SubTestEntity? GetById(int id)
    {
        const string sql = $@"
            SELECT id AS {nameof(SubTestEntity.Id)},
                   name AS {nameof(SubTestEntity.Name)},
                   test_entity_id AS {nameof(SubTestEntity.TestEntityId)}
            FROM test_sub_entities
            WHERE id = @id";

        return _connection.QueryFirstOrDefault<SubTestEntity>(sql, new { id }, _transaction);
    }

    public int UpdateName(int id, string newName)
    {
        const string sql = $@"
            UPDATE test_sub_entities
            SET name = @{nameof(newName)}
            WHERE id = @{nameof(id)}";

        return _connection.Execute(sql, new { id, newName }, _transaction);
    }

    public int Delete(int id)
    {
        const string sql = @"DELETE FROM test_sub_entities WHERE id = @id";

        return _connection.Execute(sql, new { id }, _transaction);
    }

    #endregion

    internal void SetTransaction(DbTransaction transaction)
    {
        _transaction = transaction;
    }

    internal void UnsetTransaction()
    {
        _transaction = null;
    }
}