using FluentValidation;

namespace JobBoard.Features.Auth.InitiateEmailChange;

public class InitiateEmailChangeCommandValidator : AbstractValidator<InitiateEmailChangeCommand>
{
    public InitiateEmailChangeCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}