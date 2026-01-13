namespace Gatekeeper.Demo.Api;

public sealed record LoginResponse
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? IdToken { get; init; }
    public string? Error { get; init; }
}
