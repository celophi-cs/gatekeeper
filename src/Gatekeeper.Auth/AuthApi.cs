namespace Gatekeeper.Auth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System.Security.Claims;

public class AuthApi
{
    private readonly CosmosDbService _cosmos;

    public AuthApi(CosmosDbService cosmos) => _cosmos = cosmos;

    public async Task<IResult> Login(HttpContext context, string email, string password)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Email = @email")
            .WithParameter("@email", email);
        var iter = _cosmos.UsersContainer.GetItemQueryIterator<User>(query);
        var user = (await iter.ReadNextAsync()).FirstOrDefault();

        if (user == null || user.PasswordHash != password) // In prod: hash verify
            return Results.Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };
        foreach (var role in user.Roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Results.Ok(new { message = "Logged in" });
    }

    public async Task<IResult> Logout(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Ok(new { message = "Logged out" });
    }
}

