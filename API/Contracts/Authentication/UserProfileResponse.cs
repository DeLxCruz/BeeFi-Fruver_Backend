namespace API.Contracts.Authentication;

/// <summary>
/// Response con información del perfil del usuario autenticado
/// </summary>
public record UserProfileResponse
{
    /// <summary>
    /// ID único del usuario
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Nombre(s) del usuario
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Apellido(s) del usuario
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Roles asignados al usuario
    /// </summary>
    public List<string> Roles { get; init; } = new();

    /// <summary>
    /// Indica si el usuario tiene una suscripción BeeFi activa
    /// </summary>
    public bool HasBeeFiSubscription { get; init; }

    /// <summary>
    /// Nombre del plan BeeFi (si tiene suscripción activa)
    /// </summary>
    public string? BeeFiPlanName { get; init; }

    /// <summary>
    /// Porcentaje de descuento del plan (si tiene suscripción activa)
    /// </summary>
    public decimal? DiscountPercentage { get; init; }
}
