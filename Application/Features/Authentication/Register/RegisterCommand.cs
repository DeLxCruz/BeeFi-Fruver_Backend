using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Authentication.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PhoneNumber,
        UserType Type = UserType.Cliente
    ) : IRequest<Result<RegisterResponse>>;

    public enum UserType
    {
        Cliente,
        FruverAliado,
        Empleado
    }
}