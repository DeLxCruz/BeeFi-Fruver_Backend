namespace API.Contracts.Authentication;

/// <summary>
/// Request para cerrar sesión de un usuario
/// </summary>
public record LogoutRequest
{
    /// <summary>
    /// ID del usuario que cierra sesión
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Refresh token del dispositivo actual (opcional)
    /// Si no se proporciona y RevokeAllTokens es false, se retornará error
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Si es true, revoca todos los tokens del usuario (cierra sesión en todos los dispositivos)
    /// Si es false, solo revoca el token específico proporcionado
    /// </summary>
    public bool RevokeAllTokens { get; init; } = false;
}
