using Microsoft.Azure.Cosmos;

namespace Gatekeeper.Auth;

public interface ICosmosDbProvider
{
    Container UsersContainer { get; }
    Container ClientsContainer { get; }
    Container AuthorizationsContainer { get; }
    Container TokensContainer { get; }
}
