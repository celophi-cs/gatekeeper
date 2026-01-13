using System;
using System.Text.Json.Serialization;

namespace Gatekeeper.Auth
{
    public sealed record Scope
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }
        [JsonPropertyName("name")]
        public required string Name { get; init; }
        [JsonPropertyName("description")]
        public required string Description { get; init; }
        [JsonPropertyName("resources")]
        public required string[] Resources { get; init; }
    }
}
