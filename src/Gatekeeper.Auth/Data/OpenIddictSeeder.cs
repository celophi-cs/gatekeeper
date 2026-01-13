using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Gatekeeper.Auth.Data;

public static class OpenIddictSeeder
{
    public static async Task SeedClientsAsync(IServiceProvider serviceProvider)
    {
        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        var clientSection = config.GetSection("OpenIddict:Clients:GatekeeperDevClient");
        var clientId = clientSection["ClientId"];
        var clientSecret = clientSection["ClientSecret"];
        var displayName = clientSection["DisplayName"];
        var redirectUri = clientSection["RedirectUri"];
        var postLogoutRedirectUri = clientSection["PostLogoutRedirectUri"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(redirectUri) || string.IsNullOrWhiteSpace(postLogoutRedirectUri))
        {
            throw new InvalidOperationException("Missing required OpenIddict:Clients:GatekeeperDevClient configuration values.");
        }

        if (await manager.FindByClientIdAsync(clientId) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                DisplayName = displayName,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                },
                RedirectUris = { new Uri(redirectUri) },
                PostLogoutRedirectUris = { new Uri(postLogoutRedirectUri) }
            });
        }
    }
}
