using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Webchat.AuthorizationServer
{
    public static class Startup
    {
        public static async Task CreateApplicationsAsync(AsyncServiceScope scope)
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
                        Permissions.Prefixes.Scope + "api1"
                    }
                });
            }
            if (await manager.FindByClientIdAsync("api1") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "api1",
                    ClientSecret = "secret1",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }
            if (await manager.FindByClientIdAsync("api2") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "api2",
                    ClientSecret = "secret2",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }
        }
    }
}
