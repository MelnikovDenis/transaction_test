using System.Data;
using TestProject.App.Infra.Contracts.Repos;

namespace TestProject.App.Infra.Contracts;

public interface IUnitOfWork
{
    ITestEntityRepo TestEntityRepo { get; }
    ISubTestEntityRepo TestSubEntityRepo { get; }



    #region async transactions

    Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion



    #region sync transactions

    void BeginTransaction(IsolationLevel isolation);
    void CommitTransaction();
    void RollbackTransaction();

    #endregion
}