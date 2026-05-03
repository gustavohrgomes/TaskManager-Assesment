using BallastLane.TaskManager.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskManager.API.Middleware;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail, extensions) = exception switch
        {
            DomainValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more validation errors occurred.",
                new Dictionary<string, object?>
                {
                    ["errors"] = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                }),
            TaskNotOwnedByUserException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "Task not found.",
                (Dictionary<string, object?>?)null),
            InvalidStatusTransitionException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid status transition",
                ex.Message,
                (Dictionary<string, object?>?)null),
            UnauthorizedException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "Invalid credentials.",
                (Dictionary<string, object?>?)null),
            _ => (0, (string?)null, (string?)null, (Dictionary<string, object?>?)null)
        };

        if (statusCode == 0)
            return false;

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc7807"
        };

        if (extensions is not null)
        {
            foreach (var (key, value) in extensions)
                problemDetails.Extensions[key] = value;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}
