using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Application.Exceptions.CategoryNotFoundException;

namespace LegacyArchive.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        RequestDelegate next)
    {
        _next = next;
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

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        var statusCode = exception switch
        {
            CategoryNotFoundException =>
                StatusCodes.Status404NotFound,

            ArtworkNotFoundException =>
                StatusCodes.Status404NotFound,

            CategoryAlreadyExistsException =>
                StatusCodes.Status400BadRequest,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        ProblemDetails problemDetails = new()
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Status = statusCode,
            Type = exception.GetType().Name
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails));
    }
}
