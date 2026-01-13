using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Gatekeeper.Auth;

public class CosmosDbSeeder
{
    private readonly CosmosDbService _cosmos;
    private readonly SeedDataOptions _seedData;

    public CosmosDbSeeder(CosmosDbService cosmos, IOptions<SeedDataOptions> seedDataOptions)
    {
        _cosmos = cosmos;
        _seedData = seedDataOptions.Value;
    }

    public async Task SeedAsync()
    {
        await SeedUserAsync();
        await SeedClientAsync();
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
