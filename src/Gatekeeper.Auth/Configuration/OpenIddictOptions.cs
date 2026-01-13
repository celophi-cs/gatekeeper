namespace Gatekeeper.Auth.Configuration;

public class OpenIddictOptions
{
    public string Issuer { get; set; } = "https://localhost:5001";
    public string SigningKey { get; set; } = "dev-signing-key-please-change";
    public List<ClientOptions> Clients { get; set; } = new();
}

public class ClientOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public List<string> AllowedGrantTypes { get; set; } = new();
    public List<string> RedirectUris { get; set; } = new();
}
