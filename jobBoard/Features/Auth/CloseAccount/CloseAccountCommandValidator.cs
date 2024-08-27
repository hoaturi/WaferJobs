using FluentValidation;

namespace JobBoard.Features.Auth.CloseAccount;

public class CloseAccountCommandValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}