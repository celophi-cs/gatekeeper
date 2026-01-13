namespace Gatekeeper.Auth.Models;

public class ConsentViewModel
{
    public string? ClientId { get; set; }
    public string? ClientName { get; set; }
    public IEnumerable<string> Scopes { get; set; } = Enumerable.Empty<string>();
    public string? RequestId { get; set; }
}
