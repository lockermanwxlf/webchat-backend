using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Webchat.AuthorizationServer;
using Webchat.AuthorizationServer.Data;
using Webchat.AuthorizationServer.Endpoints;

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

    // Use OpenIddict
    options.UseOpenIddict();
});

// Set up identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        // Set endpoint URIs.
        options.SetTokenEndpointUris("auth/token");
        options.SetIntrospectionEndpointUris("auth/introspect");

        // Set allowed flows
        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();

        // Set keys
        var encryptionKey = builder.Environment.IsProduction() ?
            Environment.GetEnvironmentVariable("EncryptionKey")! :
            builder.Configuration.GetValue<string>("EncryptionKey")!;
        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String(encryptionKey)));
        // The development certificate should suffice for Azure Container Apps, but
        // technically I'm supposed to make a self-signed certificate and use that instead.
        options.AddDevelopmentSigningCertificate();

        // Register ASP.NET Core host and enable passthrough for endpoints.
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();

    })
    // Add local validation as this container will host endpoints for account info.
    .AddValidation(options =>
    {
        options.UseLocalServer();

        options.UseAspNetCore();
    });

// Add authentication and authorization
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Forward HTTPS protocol as TLS connections are terminated before being routed to the container in serverless hosting.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// Ensure db exists and OpenIddict applications and scopes are registed.
using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    // Get client secrets from environment.
    var secret1 = builder.Environment.IsProduction() ?
        Environment.GetEnvironmentVariable("ApiServerClientSecret")! :
        "client-secret1";
    var secret2 = builder.Environment.IsProduction() ?
        Environment.GetEnvironmentVariable("NegotiationServerClientSecret")! :
        "client-secret2";

    await Startup.CreateApplicationsAsync(scope, secret1, secret2);
    await Startup.CreateScopesAsync(scope);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("account")
    .MapAccountEndpoints()
    .DisableAntiforgery();

app.MapGroup("auth")
    .MapAuthorizationEndpoints()
    .DisableAntiforgery();

app.Run();