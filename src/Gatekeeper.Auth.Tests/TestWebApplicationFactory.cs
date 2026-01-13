using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gatekeeper.Auth;
using Gatekeeper.Auth.Data;

namespace Gatekeeper.Auth.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Set environment variable so Program.cs uses InMemory DB for all services
        Environment.SetEnvironmentVariable("USE_INMEMORY_DB", "1");
        return base.CreateHost(builder);
    }
}
