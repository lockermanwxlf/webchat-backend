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

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();