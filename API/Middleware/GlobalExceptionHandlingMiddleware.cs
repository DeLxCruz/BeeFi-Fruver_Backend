using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones.
/// Convierte todas las excepciones no manejadas en respuestas Problem Details (RFC 9457).
/// </summary>
public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IWebHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;
    private readonly IWebHostEnvironment _environment = environment;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
                context.TraceIdentifier,
                context.Request.Path,
                context.Request.Method);

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response already started — cannot write error response for TraceId: {TraceId}", context.TraceIdentifier);
            return;
        }

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationException => BuildValidationProblem(context, validationException),
            UnauthorizedAccessException => BuildProblem(context, 401,
                "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                "Unauthorized",
                "No tienes autorización para acceder a este recurso."),
            KeyNotFoundException keyNotFoundException => BuildProblem(context, 404,
                "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                "Not Found",
                keyNotFoundException.Message),
            InvalidOperationException invalidOpException => BuildProblem(context, 400,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "Bad Request",
                invalidOpException.Message),
            ArgumentException argumentException => BuildProblem(context, 400,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "Bad Request",
                argumentException.Message),
            _ => BuildProblem(context, 500,
                "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                "Internal Server Error",
                _environment.IsDevelopment()
                    ? exception.Message
                    : "Ocurrió un error interno en el servidor.")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int, ProblemDetails) BuildProblem(
        HttpContext context, int status, string type, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        return (status, problem);
    }

    private (int, ProblemDetails) BuildValidationProblem(
        HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["errors"] = errors;

        return (400, problem);
    }
}
