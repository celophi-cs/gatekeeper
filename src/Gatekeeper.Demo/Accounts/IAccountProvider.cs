using System.Threading.Tasks;
using Gatekeeper.Demo.Api;

namespace Gatekeeper.Demo.Accounts;

public interface IAccountProvider
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
}
