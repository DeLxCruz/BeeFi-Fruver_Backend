using Application.Features.Authentication.Register;

namespace API.Contracts.Authentication;

/// <summary>
/// Request para registrar un nuevo usuario en el sistema
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// Email del usuario (único en el sistema)
    /// </summary>
    /// <example>usuario@ejemplo.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// Debe contener al menos 8 caracteres, una mayúscula, una minúscula y un número
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Nombre(s) del usuario
    /// </summary>
    /// <example>Juan Carlos</example>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Apellido(s) del usuario
    /// </summary>
    /// <example>Pérez González</example>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Número de teléfono (único en el sistema)
    /// </summary>
    /// <example>+573001234567</example>
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>
    /// Tipo de usuario (Cliente, Vendedor, Repartidor, Administrador)
    /// Por defecto es Cliente
    /// </summary>
    public UserType Type { get; init; } = UserType.Cliente;
}
