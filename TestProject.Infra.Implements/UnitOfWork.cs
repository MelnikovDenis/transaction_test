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

    private TestEntityRepo _testEntityRepo;

    public ITestEntityRepo TestEntityRepo => _testEntityRepo;

    public UnitOfWork(IOptions<PostgreSqlOptions> options, ILogger<UnitOfWork> logger)
    {
        _logger = logger;
        _options = options.Value;
        _connection = new NpgsqlConnection(_options.WorkConnectionString);
        _testEntityRepo = new TestEntityRepo(_connection);
        _connection.Open();
    }

    public async Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken cancellationToken = default)
    {
        if(_connection == null)
        {
            throw new InvalidOperationException("Невозможно начать транзакцию, соединение не установлено");
        }

        _logger.LogDebug("Старт транзакции");

       _transaction =  await _connection.BeginTransactionAsync(isolation, cancellationToken);

        _testEntityRepo.SetTransaction(_transaction);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if(_transaction == null)
        {
            throw new InvalidOperationException("Невозможно закоммитить транзакцию, т.к. транзакция не начата");
        }

        _logger.LogDebug("Коммит транзакции");

        await _transaction.CommitAsync(cancellationToken);

        _testEntityRepo.UnsetTransaction();
        await _transaction.DisposeAsync();
        _transaction = null;        
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Невозможно отменить транзакцию, т.к. транзакция не начата");
        }

        _logger.LogDebug("Отмена транзакции");

        await _transaction.RollbackAsync(cancellationToken);

        _testEntityRepo.UnsetTransaction();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    #region Dispose

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
                _transaction = null;

                _connection?.Dispose();
                _connection = null;
            }
        }
    }

    #endregion
}
