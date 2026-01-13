using System.Collections.Concurrent;

namespace Gatekeeper.Demo.Api.Accounts;

public class AccountRepository : IAccountRepository
{
    // Simple in-memory store for demo purposes
    private readonly ConcurrentDictionary<string, string> _users = new();

    public Task<bool> UserExistsAsync(string username)
        => Task.FromResult(_users.ContainsKey(username));

    public Task CreateUserAsync(string username, string password)
    {
        _users[username] = password;
        return Task.CompletedTask;
    }

    public Task<bool> ValidateUserAsync(string username, string password)
        => Task.FromResult(_users.TryGetValue(username, out var stored) && stored == password);
}
