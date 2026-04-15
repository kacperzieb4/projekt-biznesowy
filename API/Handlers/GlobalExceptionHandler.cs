using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using AttractionCatalog.Domain.Common.Exceptions;

namespace AttractionCatalog.API.Handlers;

/// <summary>
/// Converts unhandled exceptions into RFC 7807 Problem Details responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation failures have occurred."),

            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFound.Message),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // Append validation errors as extensions if applicable
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            problemDetails.Extensions["errors"] = errors;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}
