using FluentValidation;

namespace Application.Features.Reviews.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es requerido.");

        RuleFor(x => x.FruverId)
            .NotEmpty().WithMessage("El ID del fruver es requerido.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("La calificación debe estar entre 1 y 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("El comentario no puede superar 1000 caracteres.")
            .When(x => x.Comment is not null);
    }
}
