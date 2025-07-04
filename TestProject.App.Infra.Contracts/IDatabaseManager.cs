namespace TestProject.App.Infra.Contracts;

public interface IDatabaseManager
{
    public void EnsureDbCreated();

    public void EnsureDbEmpty();

    public void SeedDb();
}
