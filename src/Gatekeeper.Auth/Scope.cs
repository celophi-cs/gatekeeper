using System;

namespace Gatekeeper.Auth
{
    public record Scope
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string[] Resources { get; init; } = Array.Empty<string>();
    }
}
