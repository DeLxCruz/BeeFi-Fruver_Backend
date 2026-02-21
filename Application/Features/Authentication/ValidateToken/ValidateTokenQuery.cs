using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Authentication.ValidateToken
{
    public record ValidateTokenQuery(
        string Token
    ) : IRequest<Result<ValidateTokenResponse>>;
}