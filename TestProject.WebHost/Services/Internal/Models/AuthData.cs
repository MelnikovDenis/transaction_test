namespace TestProject.WebHost.Services.Internal.Models;

public record AuthData
{
    public required string UserName { get; init; }

    public required string UserPassword { get; init; }
}
