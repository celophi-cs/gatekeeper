using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Gatekeeper.Auth.Exceptions;

namespace Gatekeeper.Auth.Middleware;

public sealed class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;

    public ProblemDetailsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/problem+json";
            var traceId = context.TraceIdentifier;

            if (ex is GatekeeperApiException apiEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var problem400 = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = apiEx.Message,
                    Instance = context.Request.Path
                };
                problem400.Extensions["traceId"] = traceId;
                var json = JsonSerializer.Serialize(problem400, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(json);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problem500 = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "Please contact support.",
                Instance = context.Request.Path
            };
            problem500.Extensions["traceId"] = traceId;
            var jsonDefault = JsonSerializer.Serialize(problem500, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(jsonDefault);
        }
    }
}
