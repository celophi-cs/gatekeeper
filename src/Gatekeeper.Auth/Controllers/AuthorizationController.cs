using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Gatekeeper.Auth.Controllers;

[ApiController]
[AllowAnonymous]
[Route("connect/authorize")]
[ApiVersion("1.0")]
public class AuthorizationController
{
    [HttpGet]
    public IResult Get([FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext;
        // Basic validation: ensure at least 'client_id' is present in the query
        var query = context?.Request?.Query;
        if (query == null || !query.ContainsKey("client_id"))
        {
            var traceId = context?.TraceIdentifier;
            var problem = new ProblemDetails
            {
                Status = 400,
                Title = "Invalid request",
                Detail = "Missing required OpenID Connect parameters.",
                Instance = context?.Request.Path
            };
            problem.Extensions["traceId"] = traceId;
            problem.Extensions["error"] = "invalid_request";
            return Results.Problem(problem.Detail, statusCode: 400, title: problem.Title, type: null, extensions: problem.Extensions);
        }

        if (!(context.User?.Identity?.IsAuthenticated ?? false))
        {
            // Not authenticated: trigger authentication challenge (public endpoint)
            return Results.Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties(), new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme });
        }

        // Authenticated: redirect to consent UI (Gatekeeper.Demo)
        var queryString = context.Request.QueryString.Value;
        var consentUiUrl = $"https://localhost:5002/consent{queryString}"; // Adjust port/path as needed
        return Results.Redirect(consentUiUrl);
    }


}
