using System.Text.Json.Serialization;

namespace Gatekeeper.Auth;

public sealed record Client
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("clientName")]
    public required string ClientName { get; init; }
    [JsonPropertyName("redirectUris")]
    public required string[] RedirectUris { get; init; }
    [JsonPropertyName("secret")]
    public required string Secret { get; init; }
    [JsonPropertyName("typeClient")]
    public required string TypeClient { get; init; }
        [JsonPropertyName("permissions")]
        public string[] Permissions { get; set; } = Array.Empty<string>();
}
