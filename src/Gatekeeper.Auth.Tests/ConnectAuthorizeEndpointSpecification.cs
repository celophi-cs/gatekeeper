using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Gatekeeper.Auth; // Ensure correct namespace for Program

namespace Gatekeeper.Auth.Tests;

public class ConnectAuthorizeEndpointSpecification : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ConnectAuthorizeEndpointSpecification(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ConnectAuthorize_should_return_Unauthorized_when_not_authenticated()
    {
        // When: requesting GET /connect/authorize without authentication
        var response = await _client.GetAsync("/connect/authorize");
        var content = await response.Content.ReadAsStringAsync();
        System.Console.WriteLine("Response content: " + content);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
