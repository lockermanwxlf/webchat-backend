using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Webchat.AuthorizationServer.Data;

var builder = WebApplication.CreateBuilder(args);

// Register ApplicationDbContext with Cosmos provider.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Get Cosmos account endpoint from environment.
    var endpoint = builder.Environment.IsProduction() ?
        Environment.GetEnvironmentVariable("CosmosEndpoint")! :
        builder.Configuration.GetValue<string>("CosmosEndpoint")!;

    // Get Cosmos primary key from environment.
    var primaryKey = builder.Environment.IsProduction() ?
        Environment.GetEnvironmentVariable("CosmosPrimaryKey")! :
        builder.Configuration.GetValue<string>("CosmosPrimaryKey")!;
    
    // Register cosmos provider.
    options.UseCosmos(accountEndpoint: endpoint, accountKey: primaryKey, databaseName: "AuthorizationDB");
});

// Set up identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Ensure database is created in Cosmos.
using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.Run();