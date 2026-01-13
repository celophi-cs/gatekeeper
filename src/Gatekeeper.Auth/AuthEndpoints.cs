using Microsoft.AspNetCore.Identity;

namespace Gatekeeper.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/register", async (UserManager<IdentityUser> userManager, string email, string password) =>
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                return Results.Ok();
            return Results.BadRequest(result.Errors);
        });

        endpoints.MapPost("/api/login", async (SignInManager<IdentityUser> signInManager, string email, string password) =>
        {
            var result = await signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
                return Results.Ok();
            return Results.Unauthorized();
        });

        return endpoints;
    }
}
