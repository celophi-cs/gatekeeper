namespace Gatekeeper.Auth;

public record User(string Id, string Email, string PasswordHash, string[] Roles);
public record Client(string Id, string ClientName, string[] RedirectUris, string Secret, string TypeClient);
public record Authorization(string Id, string UserId, string ClientId, string[] Scopes, string Status);
public record Token(string Id, string UserId, string ClientId, string TokenType, DateTime ExpiresAt);

