using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Gatekeeper.Auth;

public class CosmosDbSeeder
{
    private readonly ICosmosDbProvider _cosmos;
    private readonly SeedDataOptions _seedData;

    public CosmosDbSeeder(ICosmosDbProvider cosmos, IOptions<SeedDataOptions> seedDataOptions)
    {
        _cosmos = cosmos;
        _seedData = seedDataOptions.Value;
    }

    public async Task SeedAsync()
    {
        await SeedUserAsync();
        await SeedClientAsync();
        await SeedScopesAsync();
    }
    private async Task SeedScopesAsync()
    {
        var defaultScopes = new[]
        {
            new Scope { Id = "openid", Name = "openid", Description = "OpenID Connect standard scope", Resources = Array.Empty<string>() },
            new Scope { Id = "profile", Name = "profile", Description = "Basic user profile information", Resources = Array.Empty<string>() },
            new Scope { Id = "email", Name = "email", Description = "User email address", Resources = Array.Empty<string>() }
        };

        foreach (var scope in defaultScopes)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", scope.Id);
            var iterator = _cosmos.ScopesContainer.GetItemQueryIterator<Scope>(query);
            var existing = (await iterator.ReadNextAsync()).FirstOrDefault();
            if (existing == null)
            {
                await _cosmos.ScopesContainer.CreateItemAsync(scope, new PartitionKey(scope.Id));
            }
        }
    }

    private async Task SeedUserAsync()
    {
        var user = _seedData.User;
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", user.Id);
        var iterator = _cosmos.UsersContainer.GetItemQueryIterator<User>(query);
        var existing = (await iterator.ReadNextAsync()).FirstOrDefault();
        if (existing == null)
        {
            var userEntity = new User
            {
                Id = user.Id,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Roles = user.Roles
            };
            await _cosmos.UsersContainer.CreateItemAsync(userEntity, new PartitionKey(userEntity.Id));
        }
    }

    private async Task SeedClientAsync()
    {
        var client = _seedData.Client;
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", client.Id);
        var iterator = _cosmos.ClientsContainer.GetItemQueryIterator<Client>(query);
        var existing = (await iterator.ReadNextAsync()).FirstOrDefault();
        if (existing == null)
        {
            var clientEntity = new Client
            {
                Id = client.Id,
                ClientName = client.ClientName,
                RedirectUris = client.RedirectUris,
                Secret = client.Secret,
                TypeClient = client.TypeClient
            };
            await _cosmos.ClientsContainer.CreateItemAsync(clientEntity, new PartitionKey(clientEntity.Id));
        }
    }
}
