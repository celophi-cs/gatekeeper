namespace Gatekeeper.Demo.Api.Accounts;

public interface IAccountRepository
{
    Task<bool> UserExistsAsync(string username);
    Task CreateUserAsync(string username, string password);
    Task<bool> ValidateUserAsync(string username, string password);
}
