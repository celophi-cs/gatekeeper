using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Gatekeeper.Auth; // Ensure correct namespace for Program

namespace Gatekeeper.Auth.Tests;

public class LoginLogoutEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginLogoutEndpointsTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AccountLogin_should_return_Ok_when_credentials_are_valid()
    {
        // Given: a registered user
        var registerResponse = await _client.PostAsJsonAsync("/api/register", new { email = "loginuser@example.com", password = "Test1234!" });
        registerResponse.EnsureSuccessStatusCode();

        // When: posting valid credentials to /account/login
        var loginResponse = await _client.PostAsJsonAsync("/account/login", new { email = "loginuser@example.com", password = "Test1234!" });

        // Then: expect Ok
        loginResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task AccountLogin_should_return_BadRequest_when_credentials_are_invalid()
    {
        // When: posting invalid credentials
        var loginResponse = await _client.PostAsJsonAsync("/account/login", new { email = "nouser@example.com", password = "WrongPass!" });

        // Then: expect BadRequest
        loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AccountLogout_should_return_Ok()
    {
        // When: posting to /account/logout
        var response = await _client.PostAsync("/account/logout", null);
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
