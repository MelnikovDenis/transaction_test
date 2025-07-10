using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestProject.WebHost.Services.Internal.Models;
using TestProject.WebHost.Services.Internal.Options;

namespace TestProject.WebHost.Services.Internal;

public class AuthService(IOptions<AuthServiceOptions> options)
{
    private readonly AuthServiceOptions _options = options.Value;

    public bool TryGetToken(AuthData authData, out string? token)
    {
        token = null;

        if(!_options.ValidUsersDict.TryGetValue(authData.UserName, out var validUserData))
        {
            return false;
        }

        if(authData.UserPassword != validUserData.UserPassword)
        {
            return false;
        }

        List<Claim> claims = [ new Claim("UserName", authData.UserName) ];

        var issuedAt = DateTime.UtcNow;

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)
            ),
            SecurityAlgorithms.HmacSha256
        );

        var jwtSecurityToken = new JwtSecurityToken(null, null, claims, issuedAt, null, signingCredentials);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

        return true;
    }
}
