using TestProject.Infra.Implements;
using TestProject.WebHost.Extensions.ServiceCollection;
using TestProject.WebHost.Extensions.ServiceProvider;
using TestProject.WebHost.Extensions.WebHost;
using TestProject.WebHost.Mappers;
using TestProject.WebHost.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAutoMapper(config => config.AddProfile<AppProfile>());

builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.SetUpMigrations(builder.Configuration);

builder.WebHost.ConfigureKestrelForGrpc(builder.Configuration);

var app = builder.Build();
app.Services.EnsureDbReady(app.Configuration);

app.MapGrpcService<TestEntityServiceImpl>();
app.MapGrpcService<SubTestEntityServiceImpl>();
app.MapGrpcReflectionService();

app.Run();