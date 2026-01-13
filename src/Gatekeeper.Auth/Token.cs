using System.Text.Json.Serialization;

namespace Gatekeeper.Auth;

public sealed record Token
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("value")]
    public required string Value { get; init; }
    [JsonPropertyName("clientId")]
    public required string ClientId { get; init; }
    [JsonPropertyName("userId")]
    public required string UserId { get; init; }
    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; init; }
}
