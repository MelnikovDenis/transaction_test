namespace TestProject.Core.Entities;

public record class TestEntity
{
    public int Id { get; set; } = -1;

    public required int Sum { get; set; }

    public required string Name { get; set; }

    public List<SubTestEntity> SubEntities { get; set; } = new List<SubTestEntity>();
}
