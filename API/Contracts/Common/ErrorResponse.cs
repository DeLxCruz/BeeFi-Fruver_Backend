namespace API.Contracts.Common;

/// <summary>
/// Respuesta de error estandarizada basada en RFC 7807 (Problem Details for HTTP APIs)
/// https://datatracker.ietf.org/doc/html/rfc7807
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// Código de error único para identificar el tipo de error
    /// Ejemplo: "User.EmailExists", "Authentication.InvalidCredentials"
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Mensaje descriptivo del error
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Timestamp cuando ocurrió el error
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// ID de trazabilidad para seguimiento en logs
    /// </summary>
    public string TraceId { get; init; }

    /// <summary>
    /// Ruta del endpoint donde ocurrió el error
    /// </summary>
    public string Path { get; init; }

    /// <summary>
    /// Errores de validación específicos (para errores 400)
    /// </summary>
    public IEnumerable<ValidationError> ValidationErrors { get; init; }

    /// <summary>
    /// Información adicional del error (solo en desarrollo)
    /// </summary>
    public object Details { get; init; }

    public ErrorResponse(
        string code,
        string message,
        string traceId = null,
        string path = null,
        IEnumerable<ValidationError> validationErrors = null,
        object details = null)
    {
        Code = code;
        Message = message;
        Timestamp = DateTime.UtcNow;
        TraceId = traceId;
        Path = path;
        ValidationErrors = validationErrors;
        Details = details;
    }
}

/// <summary>
/// Error de validación individual
/// </summary>
public record ValidationError(
    string Field,
    string Message,
    string Code = null,
    object AttemptedValue = null
);
