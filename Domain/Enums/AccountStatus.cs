namespace Domain.Enums;

/// <summary>
/// Estado de aprobación de una cuenta de usuario
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Cuenta pendiente de aprobación por un administrador
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Cuenta aprobada y activa
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Cuenta rechazada por el administrador
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Cuenta suspendida temporalmente
    /// </summary>
    Suspended = 3
}
