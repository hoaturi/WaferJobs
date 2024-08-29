using FluentValidation;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

public class VerifyAndUpdateEmailCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
{
    public VerifyAndUpdateEmailCommandValidator()
    {
        RuleFor(x => x.Pin).NotEmpty()
            .InclusiveBetween(100000, 999999)
            .WithMessage("PIN must be a 6 digit number.");
    }
}