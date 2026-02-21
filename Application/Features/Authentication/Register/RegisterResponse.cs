using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Register
{
    public record RegisterResponse(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string Role,
        string Message
    );
}