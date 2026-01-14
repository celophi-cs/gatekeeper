using Gatekeeper.Auth;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

public class CosmosApplicationStore : IOpenIddictApplicationStore<Client>
{
    private readonly ICosmosDbProvider _cosmosDbProvider;
    private readonly ILogger<CosmosApplicationStore> _logger;

    public CosmosApplicationStore(ICosmosDbProvider cosmosDbProvider, ILogger<CosmosApplicationStore> logger)
    {
        _cosmosDbProvider = cosmosDbProvider;
        _logger = logger;
    }

    // ============================
    // Required for FindByClientIdAsync / authorize flow
    // ============================

    public async ValueTask<Client?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _cosmosDbProvider.ClientsContainer.ReadItemAsync<Client>(
                id: identifier,
                partitionKey: new PartitionKey(identifier),
                cancellationToken: cancellationToken);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public ValueTask<string?> GetClientIdAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(application.Id);


    public ValueTask<string?> GetClientSecretAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(application.Secret);

    public ValueTask<string?> GetClientTypeAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(application.TypeClient);

    public ValueTask<string?> GetDisplayNameAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(application.ClientName);

    public ValueTask<bool> HasClientSecretAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(!string.IsNullOrEmpty(application.Secret));

    // ============================
    // Optional methods you can stub for now
    // ============================

    public ValueTask<long> CountAsync(CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask<long> CountAsync<TResult>(Func<IQueryable<Client>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask CreateAsync(Client application, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask DeleteAsync(Client application, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask<Client?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public IAsyncEnumerable<Client> FindByRedirectUriAsync(string uri, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public IAsyncEnumerable<Client> FindByPostLogoutRedirectUriAsync(string uri, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask<string?> GetApplicationTypeAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult<string?>(null);

    public ValueTask<ImmutableArray<string>> GetPermissionsAsync(Client application, CancellationToken cancellationToken)
    {
        if (application?.Permissions == null || application.Permissions.Length == 0)
        {
            _logger?.LogDebug("Client {ClientId} has no permissions configured.", application?.Id);
            return ValueTask.FromResult(ImmutableArray<string>.Empty);
        }

        var perms = new List<string>(application.Permissions.Length);

        foreach (var p in application.Permissions)
        {
            if (string.IsNullOrWhiteSpace(p))
                continue;

            // Normalize common seeder formats used historically to OpenIddict constants
            var trimmed = p.Trim();

            // Direct known mappings
            if (string.Equals(trimmed, "Endpoints.Authorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "authorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "endpoint.authorization", StringComparison.OrdinalIgnoreCase))
            {
                perms.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                continue;
            }

            if (string.Equals(trimmed, "Endpoints.Token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "token", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "endpoint.token", StringComparison.OrdinalIgnoreCase))
            {
                perms.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                continue;
            }

            if (string.Equals(trimmed, "GrantTypes.AuthorizationCode", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "authorization_code", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "grant_type.authorization_code", StringComparison.OrdinalIgnoreCase))
            {
                perms.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                // Also allow the corresponding response type (response_type=code)
                try
                {
                    perms.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                }
                catch
                {
                    // Some OpenIddict versions may not expose ResponseTypes; ignore if unavailable
                }
                continue;
            }

            if (string.Equals(trimmed, "GrantTypes.RefreshToken", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "refresh_token", StringComparison.OrdinalIgnoreCase))
            {
                perms.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                continue;
            }

            // Scope prefixes: handle formats like "Prefixes.Scope:openid" or "scp:openid" or just "openid"
            if (trimmed.IndexOf("prefix", StringComparison.OrdinalIgnoreCase) >= 0 && trimmed.Contains(":"))
            {
                var parts = trimmed.Split(':', 2);
                if (parts.Length == 2 && parts[1] is string scopeName && !string.IsNullOrWhiteSpace(scopeName))
                {
                    perms.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scopeName.Trim());
                    continue;
                }
            }

            // If the permission looks like a plain scope name
            if (string.Equals(trimmed, "openid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "profile", StringComparison.OrdinalIgnoreCase))
            {
                perms.Add(OpenIddictConstants.Permissions.Prefixes.Scope + trimmed.ToLowerInvariant());
                continue;
            }

            // Accept raw response type entries like "ResponseTypes.Code" or "response_type:code"
            if (trimmed.IndexOf("response", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (trimmed.IndexOf("code", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    try
                    {
                        perms.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                        continue;
                    }
                    catch
                    {
                        // ignore if not available
                    }
                }
            }

            // If it already looks like a canonical OpenIddict permission, use as-is
            perms.Add(trimmed);
        }

        var result = ImmutableArray.CreateRange(perms.Distinct(StringComparer.Ordinal));
        _logger?.LogDebug("Client {ClientId} resolved permissions: {Permissions}", application.Id, string.Join(',', result));
        return ValueTask.FromResult(result);
    }

    public ValueTask<ImmutableArray<string>> GetRequirementsAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(ImmutableArray<string>.Empty);

    public ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(ImmutableArray<string>.Empty);

    public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(ImmutableDictionary<string, JsonElement>.Empty);

    public ValueTask<Client> InstantiateAsync(CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask UpdateAsync(Client application, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public IAsyncEnumerable<Client> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<Client>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetClientIdAsync(Client application, string? identifier, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetClientSecretAsync(Client application, string? secret, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetClientTypeAsync(Client application, string? type, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetDisplayNameAsync(Client application, string? name, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetRedirectUrisAsync(Client application, ImmutableArray<string> uris, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetPostLogoutRedirectUrisAsync(Client application, ImmutableArray<string> uris, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetRequirementsAsync(Client application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetPermissionsAsync(Client application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetPropertiesAsync(Client application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetApplicationTypeAsync(Client application, string? type, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetConsentTypeAsync(Client application, string? type, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetDisplayNamesAsync(Client application, ImmutableDictionary<System.Globalization.CultureInfo, string> names, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask SetJsonWebKeySetAsync(Client application, JsonWebKeySet? set, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult<JsonWebKeySet?>(null);

    public ValueTask<string?> GetConsentTypeAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult<string?>(null);

    public ValueTask<string?> GetIdAsync(Client application, CancellationToken cancellationToken)
        => ValueTask.FromResult(application.Id);

    public ValueTask<TResult?> GetAsync<TState, TResult>(Func<IQueryable<Client>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(Client application, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(Client application, CancellationToken cancellationToken)
    {
        // Convert the string[] from your Client to ImmutableArray<string>
        var uris = application.RedirectUris != null
            ? ImmutableArray.Create(application.RedirectUris)
            : ImmutableArray<string>.Empty;

        return ValueTask.FromResult(uris);
    }

    public ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(Client application, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask SetSettingsAsync(Client application, ImmutableDictionary<string, string> settings, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
