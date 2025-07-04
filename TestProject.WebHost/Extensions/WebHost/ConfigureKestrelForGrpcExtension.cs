using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace TestProject.WebHost.Extensions.WebHost;

public static class ConfigureKestrelForGrpcExtension
{
    public static IWebHostBuilder ConfigureKestrelForGrpc(this IWebHostBuilder webHost, IConfiguration configuration)
    {
        webHost.ConfigureKestrel(options =>
        {
            options.ConfigureEndpointDefaults(listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        return webHost;
    }
}
