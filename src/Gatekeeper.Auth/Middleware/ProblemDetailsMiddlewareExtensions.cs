using Microsoft.AspNetCore.Builder;

namespace Gatekeeper.Auth.Middleware;

public static class ProblemDetailsMiddlewareExtensions
{
    public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ProblemDetailsMiddleware>();
    }
}
