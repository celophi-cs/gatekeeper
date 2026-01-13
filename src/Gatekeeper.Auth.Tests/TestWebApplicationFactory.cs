using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Gatekeeper.Auth.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Set environment variable so Program.cs uses InMemory DB for all services
        Environment.SetEnvironmentVariable("USE_INMEMORY_DB", "1");
        return base.CreateHost(builder);
    }

    public TestWebApplicationFactory()
    {
        // Ensure requests are sent as HTTPS so OpenIddict does not reject them in tests
        ClientOptions.BaseAddress = new Uri("https://localhost");
    }
}
