namespace TestProject.Core.Entities;

public record class TestEntity
{
    public required int Id { get; set; }

    public required int Sum { get; set; }

    public required string Name { get; set; }
}
