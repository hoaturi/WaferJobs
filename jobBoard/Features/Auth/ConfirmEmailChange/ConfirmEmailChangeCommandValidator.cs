using FluentValidation;
using JobBoard.Common.Constants;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

public class VerifyAndUpdateEmailCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
{
    public VerifyAndUpdateEmailCommandValidator()
    {
        RuleFor(x => x.Pin).NotEmpty()
            .InclusiveBetween(PinConstants.MinValue, PinConstants.MaxValue);
    }
}