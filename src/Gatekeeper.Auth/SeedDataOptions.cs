namespace Gatekeeper.Auth;

public class SeedDataOptions
{
    public SeedUserOptions User { get; set; } = new();
    public SeedClientOptions Client { get; set; } = new();
}
