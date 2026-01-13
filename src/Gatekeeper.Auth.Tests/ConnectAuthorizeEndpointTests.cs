using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Gatekeeper.Auth; // Ensure correct namespace for Program

namespace Gatekeeper.Auth.Tests;

public class ConnectAuthorizeEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ConnectAuthorizeEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ConnectAuthorize_should_return_Unauthorized_when_not_authenticated()
    {
        // When: posting to /connect/authorize without authentication
        var response = await _client.PostAsync("/connect/authorize", null);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
