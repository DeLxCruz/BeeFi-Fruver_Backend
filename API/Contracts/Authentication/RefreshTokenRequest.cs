namespace API.Contracts.Authentication;

/// <summary>
/// Request para renovar un JWT usando un refresh token
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Refresh token previamente emitido
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
}
