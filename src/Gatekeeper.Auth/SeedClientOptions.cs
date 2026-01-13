namespace Gatekeeper.Auth;

public class SeedClientOptions
{
    public string Id { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string[] RedirectUris { get; set; } = [];
    public string Secret { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;
}
