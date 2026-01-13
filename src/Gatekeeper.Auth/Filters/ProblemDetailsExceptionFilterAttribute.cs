using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gatekeeper.Auth.Exceptions;

namespace Gatekeeper.Auth.Filters;

public sealed class ProblemDetailsExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context == null || context.HttpContext == null)
            return;

        var httpContext = context.HttpContext;
        var traceId = httpContext.TraceIdentifier;

        if (context.Exception is GatekeeperApiException apiEx)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = apiEx.Message,
                Instance = httpContext.Request.Path
            };
            problem.Extensions["traceId"] = traceId;

            context.Result = new ObjectResult(problem)
            {
                StatusCode = StatusCodes.Status400BadRequest,
                DeclaredType = typeof(ProblemDetails)
            };

            context.ExceptionHandled = true;
            return;
        }

        var problemDefault = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "Please contact support.",
            Instance = httpContext.Request.Path
        };
        problemDefault.Extensions["traceId"] = traceId;

        context.Result = new ObjectResult(problemDefault)
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            DeclaredType = typeof(ProblemDetails)
        };

        context.ExceptionHandled = true;
    }
}
