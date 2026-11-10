using LegacyArchive.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LegacyArchive.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context,
                ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            CategoryNotFoundException =>
                StatusCodes.Status404NotFound,

            ArtworkNotFoundException =>
                StatusCodes.Status404NotFound,

            CategoryAlreadyExistsException =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = statusCode == 500 ? "An unexpected error occurred." : exception.Message,
            Status = statusCode,
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
