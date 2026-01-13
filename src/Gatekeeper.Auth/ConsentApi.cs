using Gatekeeper.Auth.Extensions;
using Microsoft.Azure.Cosmos;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace Gatekeeper.Auth;

public class ConsentApi
{
    private readonly CosmosDbService _cosmos;

    public ConsentApi(CosmosDbService cosmos) => _cosmos = cosmos;

    public async Task<IResult> GetConsent(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest();
        if (request == null || !(context.User?.Identity?.IsAuthenticated ?? false))
            return Results.Unauthorized();

        var clientQuery = new QueryDefinition("SELECT * FROM c WHERE c.Id = @id")
            .WithParameter("@id", request.ClientId);
        var client = (await _cosmos.ClientsContainer.GetItemQueryIterator<Client>(clientQuery)
            .ReadNextAsync()).FirstOrDefault();

        return Results.Json(new
        {
            ClientId = client?.Id,
            ClientName = client?.ClientName,
            Scopes = request.GetScopes()
        });
    }

    public async Task<IResult> ApproveConsent(HttpContext context, ConsentApprovalDto dto)
    {
        if (!(context.User?.Identity?.IsAuthenticated ?? false))
            return Results.Unauthorized();

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var auth = new Authorization
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            ClientId = dto.ClientId,
            Scopes = dto.Scopes,
            Status = "granted"
        };

        await _cosmos.AuthorizationsContainer.CreateItemAsync(auth, new PartitionKey(auth.UserId));

        // Sign in with claims so OpenIddict issues code
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));

        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}

public record ConsentApprovalDto(string ClientId, string[] Scopes);

