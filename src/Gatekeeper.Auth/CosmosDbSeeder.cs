using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Gatekeeper.Auth;

public class CosmosDbSeeder
{
    private readonly CosmosDbService _cosmos;

    public CosmosDbSeeder(CosmosDbService cosmos)
    {
        _cosmos = cosmos;
    }

    public async Task SeedAsync()
    {
        await SeedUserAsync();
        await SeedClientAsync();
    }

    private async Task SeedUserAsync()
    {
        var userId = "user1";
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", userId);
        var iterator = _cosmos.UsersContainer.GetItemQueryIterator<User>(query);
        var existing = (await iterator.ReadNextAsync()).FirstOrDefault();
        if (existing == null)
        {
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = "Password123!",
                Roles = new[] { "User" }
            };
                await _cosmos.UsersContainer.CreateItemAsync(user, new PartitionKey(user.Id));
            }
    }

    private async Task SeedClientAsync()
    {
        var clientId = "client1";
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", clientId);
        var iterator = _cosmos.ClientsContainer.GetItemQueryIterator<Client>(query);
        var existing = (await iterator.ReadNextAsync()).FirstOrDefault();
        if (existing == null)
        {
            var client = new Client
            {
                Id = clientId,
                ClientName = "BlazorWasm",
                RedirectUris = new[] { "https://localhost:5003/authentication/login-callback" },
                Secret = "secret",
                TypeClient = "public"
            };
            await _cosmos.ClientsContainer.CreateItemAsync(client, new PartitionKey(client.Id));
        }
    }
}
