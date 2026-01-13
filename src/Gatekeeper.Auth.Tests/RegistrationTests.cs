using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Gatekeeper.Auth.Data;
using Microsoft.EntityFrameworkCore;

namespace Gatekeeper.Auth.Tests;

public class RegistrationTests
{
    [Fact]
    public async Task CanRegisterUser()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<Gatekeeper.Auth.Data.AuthDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<Gatekeeper.Auth.Data.AuthDbContext>()
            .AddDefaultTokenProviders();
        var provider = services.BuildServiceProvider();
        var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

        var result = await userManager.CreateAsync(new IdentityUser { UserName = "test@example.com", Email = "test@example.com" }, "Test1234!");
        Assert.True(result.Succeeded);
    }
}
