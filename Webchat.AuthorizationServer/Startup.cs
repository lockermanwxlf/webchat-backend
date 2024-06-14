using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Webchat.AuthorizationServer
{
    public static class Startup
    {
        public static async Task CreateApplicationsAsync(AsyncServiceScope scope, string api1Secret, string api2Secret)
        {
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            if (await manager.FindByClientIdAsync("webapp") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "webapp",
                    ClientType = ClientTypes.Public,
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Scopes.Profile,
                        Permissions.Prefixes.Scope + "api1",
                        Permissions.Prefixes.Scope + "api2"
                    }
                });
            }
            if (await manager.FindByClientIdAsync("api_server") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "api_server",
                    ClientSecret = api1Secret,
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }
            if (await manager.FindByClientIdAsync("negotiation_server") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "negotiation_server",
                    ClientSecret = api2Secret,
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }
        }
        public static async Task CreateScopesAsync(AsyncServiceScope scope) 
        {
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            if (await manager.FindByNameAsync("api1") is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "api1",
                    Resources =
                    {
                        "api_server"
                    }
                });
            }
            if (await manager.FindByNameAsync("api2") is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "api2",
                    Resources =
                    {
                        "negotiation_server"
                    }
                });
            }
        }
    }
}
