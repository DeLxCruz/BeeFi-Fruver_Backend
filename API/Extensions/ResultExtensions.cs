using Domain.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

/// <summary>
/// Extiende Result para convertir fallos en respuestas HTTP estándar (Problem Details RFC 9457).
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot create ProblemDetails from a successful result.");

        var statusCode = MapErrorToStatusCode(result.Error.Code);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = result.Error.Message,
            Type = GetType(statusCode)
        };

        problemDetails.Extensions["errorCode"] = result.Error.Code;

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }

    private static int MapErrorToStatusCode(string errorCode)
    {
        if (errorCode.Contains("NotFound"))
            return StatusCodes.Status404NotFound;

        if (errorCode.Contains("AlreadyExists") || errorCode.Contains("Conflict"))
            return StatusCodes.Status409Conflict;

        if (errorCode.Contains("NotOwner") || errorCode.Contains("Forbidden"))
            return StatusCodes.Status403Forbidden;

        if (errorCode.Contains("Insufficient") || errorCode.Contains("Invalid") || errorCode.Contains("Cannot"))
            return StatusCodes.Status422UnprocessableEntity;

        return StatusCodes.Status400BadRequest;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        _ => "Bad Request"
    };

    private static string GetType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        422 => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.5.1"
    };
}
