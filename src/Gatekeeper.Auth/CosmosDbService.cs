namespace Gatekeeper.Auth;

using Microsoft.Azure.Cosmos;

public class CosmosDbService
{
    private readonly CosmosClient _client;
    private readonly Database _db;
    public Container UsersContainer { get; }
    public Container ClientsContainer { get; }
    public Container AuthorizationsContainer { get; }
    public Container TokensContainer { get; }

    public CosmosDbService(IConfiguration config)
    {
        var endpoint = config["Cosmos:AccountEndpoint"];
        var key = config["Cosmos:AccountKey"];
        var options = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase // Use camelCase for property names
            }
        };
        _client = new CosmosClient(endpoint, key, options);
        _db = _client.GetDatabase("OpenIdDB");

        UsersContainer = _db.GetContainer("Users");
        ClientsContainer = _db.GetContainer("Clients");
        AuthorizationsContainer = _db.GetContainer("Authorizations");
        TokensContainer = _db.GetContainer("Tokens");
    }
}

