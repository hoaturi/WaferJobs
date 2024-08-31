using FluentValidation;
using JobBoard.Common.Constants;

namespace JobBoard.Features.Auth.ChangeEmail.CompleteEmailChange;

public class VerifyAndUpdateEmailCommandValidator : AbstractValidator<CompleteEmailChangeCommand>
{
    public VerifyAndUpdateEmailCommandValidator()
    {
        RuleFor(x => x.Pin).NotEmpty()
            .InclusiveBetween(PinConstants.MinValue, PinConstants.MaxValue);
    }
}