using TestProject.Infra.Implements;
using TestProject.WebHost.Extensions.ServiceCollection;
using TestProject.WebHost.Extensions.ServiceProvider;
using TestProject.WebHost.Extensions.WebHost;
using TestProject.WebHost.Interceptors;
using TestProject.WebHost.Mappers;
using TestProject.WebHost.Services;
using TestProject.WebHost.Services.Internal;
using TestProject.WebHost.Services.Internal.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthServiceOptions>(builder.Configuration.GetRequiredSection(nameof(AuthServiceOptions)));
builder.Services.AddTransient<AuthService>();

builder.Services.AddSingleton<RequestCounterService>();

builder.Services.AddJwt(builder.Configuration);
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<RequestCounterInterceptor>();
});
builder.Services.AddGrpcReflection();
builder.Services.AddAutoMapper(config => config.AddProfile<AppProfile>());

builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.SetUpMigrations(builder.Configuration);

builder.WebHost.ConfigureKestrelForGrpc(builder.Configuration);

var app = builder.Build();
app.Services.EnsureDbReady(app.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<TestEntityServiceImpl>();
app.MapGrpcService<SubTestEntityServiceImpl>();
app.MapGrpcService<AuthGrpcServiceImpl>();
app.MapGrpcReflectionService();

app.Run();