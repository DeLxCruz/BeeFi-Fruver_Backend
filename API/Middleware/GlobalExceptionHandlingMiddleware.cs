using API.Contracts.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// Captura todas las excepciones no manejadas y las convierte en respuestas HTTP estandarizadas
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
                "An unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Path);

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // ✅ Solo modificar headers/response si NO ha empezado
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot modify headers/body for exception handling");
            return;
        }

        context.Response.ContentType = "application/json";

        var (statusCode, errorResponse) = exception switch
        {
            ValidationException validationException => HandleValidationException(
                context,
                validationException),

            UnauthorizedAccessException => HandleUnauthorizedException(context),

            KeyNotFoundException notFoundException => HandleNotFoundException(
                context,
                notFoundException),

            InvalidOperationException invalidOperationException => HandleInvalidOperationException(
                context,
                invalidOperationException),

            ArgumentException argumentException => HandleArgumentException(
                context,
                argumentException),

            _ => HandleUnknownException(context, exception)
        };

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(json);
    }

    private (HttpStatusCode, ErrorResponse) HandleValidationException(
        HttpContext context,
        ValidationException exception)
    {
        var validationErrors = exception.Errors
            .Select(error => new ValidationError(
                Field: error.PropertyName,
                Message: error.ErrorMessage,
                Code: error.ErrorCode,
                AttemptedValue: error.AttemptedValue))
            .ToList();

        var errorResponse = new ErrorResponse(
            code: "Validation.Failed",
            message: "Uno o más errores de validación ocurrieron",
            traceId: context.TraceIdentifier,
            path: context.Request.Path,
            validationErrors: validationErrors,
            details: _environment.IsDevelopment() ? exception.StackTrace : null);

        return (HttpStatusCode.BadRequest, errorResponse);
    }

    private (HttpStatusCode, ErrorResponse) HandleUnauthorizedException(HttpContext context)
    {
        var errorResponse = new ErrorResponse(
            code: "Authorization.Unauthorized",
            message: "No tienes autorización para acceder a este recurso",
            traceId: context.TraceIdentifier,
            path: context.Request.Path);

        return (HttpStatusCode.Unauthorized, errorResponse);
    }

    private (HttpStatusCode, ErrorResponse) HandleNotFoundException(
        HttpContext context,
        KeyNotFoundException exception)
    {
        var errorResponse = new ErrorResponse(
            code: "Resource.NotFound",
            message: exception.Message,
            traceId: context.TraceIdentifier,
            path: context.Request.Path,
            details: _environment.IsDevelopment() ? exception.StackTrace : null);

        return (HttpStatusCode.NotFound, errorResponse);
    }

    private (HttpStatusCode, ErrorResponse) HandleInvalidOperationException(
        HttpContext context,
        InvalidOperationException exception)
    {
        var errorResponse = new ErrorResponse(
            code: "Operation.Invalid",
            message: exception.Message,
            traceId: context.TraceIdentifier,
            path: context.Request.Path,
            details: _environment.IsDevelopment() ? exception.StackTrace : null);

        return (HttpStatusCode.BadRequest, errorResponse);
    }

    private (HttpStatusCode, ErrorResponse) HandleArgumentException(
        HttpContext context,
        ArgumentException exception)
    {
        var errorResponse = new ErrorResponse(
            code: "Argument.Invalid",
            message: exception.Message,
            traceId: context.TraceIdentifier,
            path: context.Request.Path,
            details: _environment.IsDevelopment() ? exception.StackTrace : null);

        return (HttpStatusCode.BadRequest, errorResponse);
    }

    private (HttpStatusCode, ErrorResponse) HandleUnknownException(
        HttpContext context,
        Exception exception)
    {
        // No exponer detalles internos en producción
        var message = _environment.IsDevelopment()
            ? exception.Message
            : "Ocurrió un error interno en el servidor. Por favor, contacta al administrador.";

        var errorResponse = new ErrorResponse(
            code: "Server.InternalError",
            message: message,
            traceId: context.TraceIdentifier,
            path: context.Request.Path,
            details: _environment.IsDevelopment()
                ? new
                {
                    exception.Message,
                    exception.StackTrace,
                    InnerException = exception.InnerException?.Message
                }
                : null);

        return (HttpStatusCode.InternalServerError, errorResponse);
    }
}