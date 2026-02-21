using API.Contracts.Authentication;
using API.Contracts.Common;
using Application.Features.Authentication.Login;
using Application.Features.Authentication.Logout;
using Application.Features.Authentication.RefreshToken;
using Application.Features.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        ISender mediator,
        ILogger<AuthenticationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos del usuario a registrar</param>
    /// <returns>Información del usuario creado</returns>
    /// <response code="201">Usuario registrado exitosamente</response>
    /// <response code="400">Datos inválidos o email ya existe</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Type);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Registration failed for email {Email}. Error: {Error}",
                request.Email,
                result.Error.Message);

            return BadRequest(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path
            ));
        }

        _logger.LogInformation(
            "User registered successfully. UserId: {UserId}, Email: {Email}",
            result.Value.UserId,
            result.Value.Email);

        return CreatedAtAction(
            nameof(Register),
            new { id = result.Value.UserId },
            result.Value);
    }

    /// <summary>
    /// Autentica un usuario y devuelve tokens JWT
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Tokens de acceso y refresh</returns>
    /// <response code="200">Login exitoso</response>
    /// <response code="401">Credenciales inválidas</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var deviceInfo = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new LoginCommand(
            request.Email,
            request.Password,
            deviceInfo,
            ipAddress);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Login failed for email {Email}. Error: {Error}",
                request.Email,
                result.Error.Message);

            return Unauthorized(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path
            ));
        }

        _logger.LogInformation(
            "User logged in successfully. UserId: {UserId}, Email: {Email}",
            result.Value.UserId,
            result.Value.Email);

        return Ok(result.Value);
    }

    /// <summary>
    /// Renueva el JWT usando un refresh token válido
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Nuevos tokens de acceso y refresh</returns>
    /// <response code="200">Token renovado exitosamente</response>
    /// <response code="401">Refresh token inválido o expirado</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Refresh token attempt");

        var deviceInfo = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new RefreshTokenCommand(
            request.RefreshToken,
            deviceInfo,
            ipAddress);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Refresh token failed. Error: {Error}", result.Error.Message);

            return Unauthorized(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path
            ));
        }

        _logger.LogInformation(
            "Token refreshed successfully for UserId: {UserId}",
            result.Value.UserId);

        return Ok(result.Value);
    }

    /// <summary>
    /// Cierra sesión revocando el refresh token
    /// </summary>
    /// <param name="request">Información de logout</param>
    /// <returns>Confirmación de logout</returns>
    /// <response code="200">Logout exitoso</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        _logger.LogInformation(
            "Logout attempt for UserId: {UserId}, RevokeAll: {RevokeAll}",
            request.UserId,
            request.RevokeAllTokens);

        var command = new LogoutCommand(
            request.UserId,
            request.RefreshToken,
            request.RevokeAllTokens);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Logout failed for UserId: {UserId}. Error: {Error}",
                request.UserId,
                result.Error.Message);

            return BadRequest(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path
            ));
        }

        _logger.LogInformation(
            "User logged out successfully. UserId: {UserId}, TokensRevoked: {TokensRevoked}",
            request.UserId,
            result.Value.TokensRevoked);

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el perfil del usuario autenticado actual
    /// </summary>
    /// <returns>Información del usuario</returns>
    /// <response code="200">Perfil obtenido exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Aquí puedes implementar un GetUserProfile query
        return Ok(new
        {
            UserId = userId,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            Roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList()
        });
    }
}