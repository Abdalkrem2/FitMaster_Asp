using FitMaster.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FitMaster.Application.Common.Exceptions.ValidationException;

namespace FitMaster.WebApi.Middleware;

/// <summary>
/// Catches every unhandled exception that reaches the WebApi layer and turns
/// it into a consistent <see cref="ProblemDetails"/> JSON response instead of
/// letting ASP.NET Core's default (an empty 500, or a raw stack trace in dev)
/// leak out. Registered once via <c>AddExceptionHandler</c> in Program.cs.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            // Only truly unexpected failures get logged as errors + a stack trace;
            // validation/not-found are expected outcomes, not bugs.
            logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = statusCode;

        if (exception is ValidationException validationException)
        {
            var validationProblem = new ValidationProblemDetails(validationException.Errors)
            {
                Status = statusCode,
                Title = title,
            };
            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
            return true;
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message,
        };
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
        NotFoundException => (StatusCodes.Status404NotFound, "The requested resource was not found."),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
    };
}
