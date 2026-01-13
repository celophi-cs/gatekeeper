using System.Text.Json.Serialization;

namespace Gatekeeper.Auth;

public sealed record Authorization
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("userId")]
    public required string UserId { get; init; }

    [JsonPropertyName("clientId")]
    public required string ClientId { get; init; }

    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }
}
