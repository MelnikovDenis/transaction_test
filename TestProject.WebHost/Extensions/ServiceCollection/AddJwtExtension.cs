using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestProject.WebHost.Services.Internal.Options;

namespace TestProject.WebHost.Extensions.ServiceCollection;

public static class AddJwtExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var authServiceOptions = configuration.GetRequiredSection(nameof(AuthServiceOptions)).Get<AuthServiceOptions>()
            ?? throw new InvalidOperationException($"Не найдена конфигурация '{nameof(AuthServiceOptions)}' в настройках приложения.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authServiceOptions.SecretKey))
                };
            });

        services.AddAuthorization();

        return services;
    }
}
