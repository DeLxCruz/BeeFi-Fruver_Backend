using FluentValidation;

namespace Application.Features.ReturnRequests.ReviewReturnRequest;

public class ReviewReturnRequestCommandValidator : AbstractValidator<ReviewReturnRequestCommand>
{
    public ReviewReturnRequestCommandValidator()
    {
        RuleFor(x => x.ReturnRequestId)
            .NotEmpty().WithMessage("El ID de la solicitud es requerido.");
    }
}
