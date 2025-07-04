namespace TestProject.Core.Entities;

public record class SubTestEntity
{    
    public int Id { get; set; } = -1;

    public required string Name { get; set; }

    public required int TestEntityId { get; set; }

}