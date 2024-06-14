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

// Forward HTTPS protocol as TLS connections are terminated before being routed to the container in serverless hosting.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.Run();