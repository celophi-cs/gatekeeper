using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Gatekeeper.Auth.Data;

public static class OpenIddictSeeder
{
    public static async Task SeedClientsAsync(IServiceProvider serviceProvider)
    {
        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Example: Seed a default client if it doesn't exist
        if (await manager.FindByClientIdAsync("gatekeeper-dev-client") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "gatekeeper-dev-client",
                ClientSecret = "dev-secret", // Use a secure secret in production
                DisplayName = "Gatekeeper Dev Client",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                },
                RedirectUris = { new Uri("https://localhost:5001/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("https://localhost:5001/signout-callback-oidc") }
            });
        }
    }
}
