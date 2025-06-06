using System.Data;
using TestProject.App.Infra.Contracts;
using TestProject.Core.Entities;
using TestProject.Infra.Implements;
using TestProject.WebHost.Extensions.ServiceCollection;
using TestProject.WebHost.Extensions.ServiceProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.SetUpMigrations(builder.Configuration);

var app = builder.Build();
app.Services.EnsureDbReady(app.Configuration);

using (var scope = app.Services.CreateAsyncScope())
{

    await Task.Run(async () => 
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var alice = new TestEntity { Id = 1, Name = "Alice", Sum = 0 };

        await uof.TestEntityRepo.CreateAsync(alice);
    });

    var task1 = Task.Run(async () => 
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await uof.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        await uof.TestEntityRepo.AddSumAsync(1, 100);

        await Task.Delay(100);

        await uof.CommitTransactionAsync();
    });

    var task2 = Task.Run(async () =>
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await uof.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        await uof.TestEntityRepo.AddSumAsync(1, 200);

        await Task.Delay(200);

        await uof.CommitTransactionAsync();

    });

    await Task.WhenAll(task1, task2);
}

// app.Run();