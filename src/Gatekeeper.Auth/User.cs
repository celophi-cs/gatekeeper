using System.Text.Json.Serialization;

namespace Gatekeeper.Auth;

public sealed record User
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("email")]
    public required string Email { get; init; }
    [JsonPropertyName("passwordHash")]
    public required string PasswordHash { get; init; }
    [JsonPropertyName("roles")]
    public required string[] Roles { get; init; }
}
