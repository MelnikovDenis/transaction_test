using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Data.Common;
using TestProject.App.Infra.Contracts;
using TestProject.App.Infra.Contracts.Repos;
using TestProject.Infra.Implements.Options;
using TestProject.Infra.Implements.Repos;

namespace TestProject.Infra.Implements;

internal class UnitOfWork : IDisposable, IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly PostgreSqlOptions _options;

    private DbConnection? _connection;
    private DbTransaction? _transaction;

    private readonly TestEntityRepo _testEntityRepo;
    private readonly SubTestEntityRepo _subTestEntityRepo;

    public ITestEntityRepo TestEntityRepo => _testEntityRepo;
    public ISubTestEntityRepo SubTestEntityRepo => _subTestEntityRepo;

    public UnitOfWork(IOptions<PostgreSqlOptions> options, ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _options = options.Value;
        _connection = new NpgsqlConnection(_options.WorkConnectionString);
        _testEntityRepo = new TestEntityRepo(_connection);
        _subTestEntityRepo = new SubTestEntityRepo(_connection);
        _connection.Open();
    }

    #region async transactions

    public async Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken cancellationToken = default)
    {
        EnsureConnection();

        _logger.LogDebug("Старт транзакции (async)");

        _transaction = await _connection!.BeginTransactionAsync(isolation, cancellationToken);

        _testEntityRepo.SetTransaction(_transaction);
        _subTestEntityRepo.SetTransaction(_transaction);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        EnsureTransactionStarted();

        _logger.LogDebug("Коммит транзакции (async)");

        await _transaction!.CommitAsync(cancellationToken);

        ClearTransaction();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        EnsureTransactionStarted();

        _logger.LogDebug("Откат транзакции (async)");

        await _transaction!.RollbackAsync(cancellationToken);

        ClearTransaction();
    }

    #endregion

    #region sync transactions

    public void BeginTransaction(IsolationLevel isolation)
    {
        EnsureConnection();

        _logger.LogDebug("Старт транзакции (sync)");

        _transaction = _connection!.BeginTransaction(isolation);

        _testEntityRepo.SetTransaction(_transaction);
        _subTestEntityRepo.SetTransaction(_transaction);
    }

    public void CommitTransaction()
    {
        EnsureTransactionStarted();

        _logger.LogDebug("Коммит транзакции (sync)");

        _transaction!.Commit();

        ClearTransaction();
    }

    public void RollbackTransaction()
    {
        EnsureTransactionStarted();

        _logger.LogDebug("Откат транзакции (sync)");

        _transaction!.Rollback();

        ClearTransaction();
    }

    #endregion

    #region helpers

    private void EnsureConnection()
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Невозможно начать транзакцию, соединение не установлено");
        }
    }

    private void EnsureTransactionStarted()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Транзакция не начата");
        }
    }

    private void ClearTransaction()
    {
        _testEntityRepo.UnsetTransaction();
        _subTestEntityRepo.UnsetTransaction();

        _transaction?.Dispose();
        _transaction = null;
    }

    #endregion

    #region dispose

    ~UnitOfWork()
    {
        Dispose(false);
    }

    private bool _isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
                _transaction = null;
                _connection = null;
            }
        }
    }

    #endregion
}