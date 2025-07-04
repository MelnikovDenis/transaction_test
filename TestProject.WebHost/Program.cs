using TestProject.Infra.Implements;
using TestProject.WebHost.Extensions.ServiceCollection;
using TestProject.WebHost.Extensions.ServiceProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.SetUpMigrations(builder.Configuration);

var app = builder.Build();
app.Services.EnsureDbReady(app.Configuration);

app.Run();