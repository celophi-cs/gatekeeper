using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Gatekeeper.Demo.Api;

namespace Gatekeeper.Demo.Accounts;

public class AccountProvider : IAccountProvider
{
    private readonly HttpClient _httpClient;
    public AccountProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }
        else
        {
            return new LoginResponse { Success = false, Error = await response.Content.ReadAsStringAsync() };
        }
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}
