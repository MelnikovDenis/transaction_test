using TestProject.Core.Entities;

namespace TestProject.App.Infra.Contracts.Repos;

public interface ISubTestEntityRepo
{
    #region async contracts

    public Task<int> CreateAsync(
        SubTestEntity entity,
        CancellationToken cancellationToken = default);

    public Task<SubTestEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    public Task<int> UpdateNameAsync(
        int id,
        string newName,
        CancellationToken cancellationToken = default);

    public Task<int> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    #endregion

    #region sync contracts

    public int Create(SubTestEntity entity);

    public SubTestEntity? GetById(int id);

    public int UpdateName(int id, string newName);

    public int Delete(int id);

    #endregion
}