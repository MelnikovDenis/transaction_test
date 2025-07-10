using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestProject.WebHost.Services.Internal.Models;

namespace TestProject.WebHost.Services.Internal.Options;

public record class AuthServiceOptions
{    
    public IReadOnlyDictionary<string, AuthData> ValidUsersDict => ValidUsers.ToDictionary(x => x.UserName, x => x);
    
    public required List<AuthData> ValidUsers { get; init; }

    public required string SecretKey { get; init; }
    
}