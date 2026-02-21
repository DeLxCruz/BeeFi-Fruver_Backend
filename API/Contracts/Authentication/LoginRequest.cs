namespace API.Contracts.Authentication;

/// <summary>
/// Request para autenticar un usuario
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    /// <example>usuario@ejemplo.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
