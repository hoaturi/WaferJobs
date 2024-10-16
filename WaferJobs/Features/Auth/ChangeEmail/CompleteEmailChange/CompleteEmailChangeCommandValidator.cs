using FluentValidation;

namespace WaferJobs.Features.Auth.ChangeEmail.CompleteEmailChange;

public class VerifyAndUpdateEmailCommandValidator : AbstractValidator<CompleteEmailChangeCommand>
{
    public VerifyAndUpdateEmailCommandValidator()
    {
        RuleFor(x => x.Pin).NotEmpty();
    }
}