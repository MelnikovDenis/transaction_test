using TestProject.Core.Entities;

namespace TestProject.Infra.Implements;

internal static class SeedGenerator
{
    private static readonly Random random = new Random();

    private readonly static IReadOnlyList<string> SeedingAdjectives = [
        "Brave",
        "Silent",
        "Clever",
        "Bright",
        "Gentle",
        "Fierce",
        "Mighty",
        "Swift",
        "Happy",
        "Calm"
    ];

    private readonly static IReadOnlyList<string> SeedingNouns = [
        "Falcon",
        "River",
        "Mountain",
        "Shadow",
        "Lion",
        "Forest",
        "Wolf",
        "Storm",
        "Ocean",
        "Flame"
    ];

    private readonly static (int, int) SumBorders = (-1000, 1001);

    public static TestEntity GetRandomTestEntity()
    {
        return new TestEntity
        {
            Sum = random.Next(SumBorders.Item1, SumBorders.Item2),
            Name = SeedingAdjectives[random.Next(0, SeedingAdjectives.Count)] + " " + SeedingNouns[random.Next(0, SeedingNouns.Count)]
        };
    }

    public static SubTestEntity GetRandomSubTestEntity(int testEntityId)
    {
        return new SubTestEntity
        {
            TestEntityId = testEntityId,
            Name = SeedingAdjectives[random.Next(0, SeedingAdjectives.Count)] + " " + SeedingNouns[random.Next(0, SeedingNouns.Count)]
        };
    }

    public static int GetRandomSubTestEntityCount(int maxCount)
    {
        return random.Next(0, maxCount);
    }
}
