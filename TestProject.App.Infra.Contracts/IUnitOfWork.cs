using System.Data;
using TestProject.App.Infra.Contracts.Repos;

namespace TestProject.App.Infra.Contracts;

public interface IUnitOfWork
{
    ITestEntityRepo TestEntityRepo { get; }

    public Task BeginTransactionAsync(IsolationLevel isolation, CancellationToken cancellationToken = default);

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
