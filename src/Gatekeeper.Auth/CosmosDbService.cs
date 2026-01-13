namespace Gatekeeper.Auth;

using Microsoft.Azure.Cosmos;

using Microsoft.Extensions.Options;

public class CosmosDbProvider : ICosmosDbProvider
{
    private readonly CosmosClient _client;
    private readonly Database _db;
    public Container UsersContainer { get; }
    public Container ClientsContainer { get; }
    public Container AuthorizationsContainer { get; }
    public Container TokensContainer { get; }

    public CosmosDbProvider(IOptions<CosmosOptions> options)
    {
        var cosmosOptions = options.Value;
        var endpoint = cosmosOptions.AccountEndpoint;
        var key = cosmosOptions.AccountKey;
        var clientOptions = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase // Use camelCase for property names
            }
        };
        _client = new CosmosClient(endpoint, key, clientOptions);
        _db = _client.GetDatabase("OpenIdDB");

        UsersContainer = _db.GetContainer("Users");
        ClientsContainer = _db.GetContainer("Clients");
        AuthorizationsContainer = _db.GetContainer("Authorizations");
        TokensContainer = _db.GetContainer("Tokens");
    }
}

