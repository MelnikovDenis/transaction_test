using Dapper;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using TestProject.App.Infra.Contracts;
using TestProject.App.Infra.Contracts.Repos;
using TestProject.Core.Entities;

namespace TestProject.Infra.Implements.Repos;

internal class TestEntityRepo(DbConnection connection) : ITestEntityRepo
{
    private DbTransaction? _transaction;

    private readonly DbConnection _connection = connection;

    #region async implements

    public async Task<int> CreateAsync(TestEntity entity, CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"
            INSERT INTO test_entities (sum, name)
            VALUES (@{nameof(TestEntity.Sum)}, @{nameof(TestEntity.Name)})
            RETURNING id;";

        var command = new CommandDefinition(
            sqlQuery,
            new { entity.Sum, entity.Name },
            _transaction,
            cancellationToken: cancellationToken);

        var generatedId = await _connection.ExecuteScalarAsync<int>(command);
        return generatedId;
    }

    public async Task<TestEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _connection.QueryFirstOrDefaultAsync<TestEntity>(
            new CommandDefinition(
                @$"SELECT id AS {nameof(TestEntity.Id)}, 
                          sum AS {nameof(TestEntity.Sum)}, 
                          name AS {nameof(TestEntity.Name)} 
                   FROM test_entities 
                   WHERE id = @id",
                new { id },
                _transaction,
                cancellationToken: cancellationToken));

        if (entity is null)
        {
            return null;
        }            

        var subEntities = (await _connection.QueryAsync<SubTestEntity>(
            new CommandDefinition(
                @$"SELECT id AS {nameof(SubTestEntity.Id)}, 
                          name AS {nameof(SubTestEntity.Name)}, 
                          test_entity_id AS {nameof(SubTestEntity.TestEntityId)} 
                   FROM test_sub_entities 
                   WHERE test_entity_id = @id",
                new { id },
                _transaction,
                cancellationToken: cancellationToken))).ToList();

        entity.SubEntities = subEntities;

        return entity;
    }

    public async Task<int> AddSumAsync(int id, int sumToAdd, CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"
            UPDATE test_entities 
            SET sum = sum + @{nameof(sumToAdd)} 
            WHERE id = @{nameof(id)}";

        return await _connection.ExecuteAsync(
            new CommandDefinition(sqlQuery, new { id, sumToAdd }, _transaction, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sqlQuery = $@"DELETE FROM test_entities WHERE id = @{nameof(id)}";

        return await _connection.ExecuteAsync(
            new CommandDefinition(sqlQuery, new { id }, _transaction, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<TestEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string entitySql = $@"
            SELECT id AS {nameof(TestEntity.Id)},
                   sum AS {nameof(TestEntity.Sum)},
                   name AS {nameof(TestEntity.Name)}
            FROM test_entities";

        const string subEntitySql = $@"
            SELECT id AS {nameof(SubTestEntity.Id)},
                   name AS {nameof(SubTestEntity.Name)},
                   test_entity_id AS {nameof(SubTestEntity.TestEntityId)}
            FROM test_sub_entities";

        var command1 = new CommandDefinition(entitySql, transaction: _transaction, cancellationToken: cancellationToken);
        var command2 = new CommandDefinition(subEntitySql, transaction: _transaction, cancellationToken: cancellationToken);

        var entities = (await _connection.QueryAsync<TestEntity>(command1)).ToList();
        var subEntities = (await _connection.QueryAsync<SubTestEntity>(command2)).ToList();

        var subEntitiesLookup = subEntities.GroupBy(se => se.TestEntityId)
                                           .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var entity in entities)
        {
            if (subEntitiesLookup.TryGetValue(entity.Id, out var subs))
            {
                entity.SubEntities = subs;
            }
        }

        return entities;
    }

    public async IAsyncEnumerable<TestEntity> GetAllAsStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @$"SELECT 
                id AS {nameof(TestEntity.Id)}, 
                sum AS {nameof(TestEntity.Sum)}, 
                name AS {nameof(TestEntity.Name)} 
            FROM test_entities",
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await using var reader = await _connection.ExecuteReaderAsync(command, CommandBehavior.SequentialAccess);

        var idOrdinal = reader.GetOrdinal(nameof(TestEntity.Id));
        var sumOrdinal = reader.GetOrdinal(nameof(TestEntity.Sum));
        var nameOrdinal = reader.GetOrdinal(nameof(TestEntity.Name));

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new TestEntity
            {
                Id = reader.GetInt32(idOrdinal),
                Sum = reader.GetInt32(sumOrdinal),
                Name = reader.GetString(nameOrdinal)
            };
        }
    }

    #endregion



    #region sync implements

    public int Create(TestEntity entity)
    {
        const string sqlQuery = $@"
            INSERT INTO test_entities (sum, name)
            VALUES (@{nameof(TestEntity.Sum)}, @{nameof(TestEntity.Name)})
            RETURNING id AS {nameof(TestEntity.Id)};";

        var generatedId = _connection.ExecuteScalar<int>(
            sqlQuery,
            new { entity.Sum, entity.Name },
            _transaction);

        return generatedId;
    }

    public TestEntity? GetById(int id)
    {
        const string entitySql = $@"
            SELECT id AS {nameof(TestEntity.Id)}, 
                   sum AS {nameof(TestEntity.Sum)}, 
                   name AS {nameof(TestEntity.Name)} 
            FROM test_entities 
            WHERE id = @id";

        var entity = _connection.QueryFirstOrDefault<TestEntity>(entitySql, new { id }, _transaction);

        if (entity is null)
        {
            return null;
        }           

        const string subEntitySql = $@"
            SELECT id AS {nameof(SubTestEntity.Id)}, 
                   name AS {nameof(SubTestEntity.Name)}, 
                   test_entity_id AS {nameof(SubTestEntity.TestEntityId)} 
            FROM test_sub_entities 
            WHERE test_entity_id = @id";

        var subEntities = _connection.Query<SubTestEntity>(subEntitySql, new { id }, _transaction).ToList();

        entity.SubEntities = subEntities;

        return entity;
    }

    public int AddSum(int id, int sumToAdd)
    {
        const string sqlQuery = $@"
            UPDATE test_entities 
            SET sum = sum + @{nameof(sumToAdd)} 
            WHERE id = @{nameof(id)}";

        return _connection.Execute(sqlQuery, new { id, sumToAdd }, _transaction);
    }

    public int Delete(int id)
    {
        const string sqlQuery = $@"DELETE FROM test_entities WHERE id = @{nameof(id)}";

        return _connection.Execute(sqlQuery, new { id }, _transaction);
    }

    public IEnumerable<TestEntity> GetAll()
    {
        const string entitySql = $@"
            SELECT id AS {nameof(TestEntity.Id)},
                   sum AS {nameof(TestEntity.Sum)},
                   name AS {nameof(TestEntity.Name)}
            FROM test_entities";

        const string subEntitySql = $@"
            SELECT id AS {nameof(SubTestEntity.Id)},
                   name AS {nameof(SubTestEntity.Name)},
                   test_entity_id AS {nameof(SubTestEntity.TestEntityId)}
            FROM test_sub_entities";

        var entities = _connection.Query<TestEntity>(entitySql, transaction: _transaction).ToList();
        var subEntities = _connection.Query<SubTestEntity>(subEntitySql, transaction: _transaction).ToList();

        var subEntitiesLookup = subEntities.GroupBy(se => se.TestEntityId)
                                           .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var entity in entities)
        {
            if (subEntitiesLookup.TryGetValue(entity.Id, out var subs))
            {
                entity.SubEntities = subs;
            }
        }

        return entities;
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
