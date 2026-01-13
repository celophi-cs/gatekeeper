namespace Gatekeeper.Demo.Api;

public sealed record LoginRequest
{
    // For OAuth2 password grant or resource owner login
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    // Optionally add: client_id, scope, grant_type, etc. for full OAuth
    public string? ClientId { get; init; }
    public string? Scope { get; init; }
    public string? GrantType { get; init; }
}
