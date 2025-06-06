using TestProject.Core.Entities;

namespace TestProject.App.Infra.Contracts.Repos;

public interface ITestEntityRepo
{
    public Task<int> CreateAsync(
        TestEntity entity,
        CancellationToken cancellationToken = default);

    public Task<TestEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    public Task<int> AddSumAsync(
       int id,
       int sumToAdd,
       CancellationToken cancellationToken = default);

    public Task<int> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
