using System.Runtime.CompilerServices;
using TestProject.Core.Entities;

namespace TestProject.App.Infra.Contracts.Repos;

public interface ITestEntityRepo
{
    #region async contracts

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

    public Task<IEnumerable<TestEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<TestEntity> GetAllAsStreamAsync(
        CancellationToken cancellationToken = default);

    #endregion



    #region sync contracts

    public int Create(TestEntity entity);

    public TestEntity? GetById(int id);

    public int AddSum(int id, int sumToAdd);

    public int Delete(int id);

    public IEnumerable<TestEntity> GetAll();

    #endregion
}
