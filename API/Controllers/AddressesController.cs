using API.Contracts.Addresses;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Addresses.CreateAddress;
using Application.Features.Addresses.DeleteAddress;
using Application.Features.Addresses.GetMyAddresses;
using Application.Features.Addresses.UpdateAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión de direcciones de entrega
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/addresses")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene las direcciones del usuario autenticado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAddresses()
    {
        var result = await _mediator.Send(new GetMyAddressesQuery());
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea una nueva dirección de entrega
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
    {
        var command = new CreateAddressCommand(
            request.ZoneId,
            request.AliasName,
            request.Street,
            request.HouseNumber,
            request.Neighborhood,
            request.Latitude,
            request.Longitude,
            request.IsDefault);

        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetMyAddresses), new { }, new { id = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza una dirección existente
    /// </summary>
    [HttpPut("{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] UpdateAddressRequest request)
    {
        var command = new UpdateAddressCommand(
            addressId,
            request.AliasName,
            request.Street,
            request.HouseNumber,
            request.Neighborhood,
            request.Latitude,
            request.Longitude,
            request.IsDefault);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Dirección actualizada" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Elimina (soft delete) una dirección
    /// </summary>
    [HttpDelete("{addressId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid addressId)
    {
        var command = new DeleteAddressCommand(addressId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Dirección eliminada" });

        return result.ToProblemDetails();
    }
}
