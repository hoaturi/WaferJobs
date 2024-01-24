using FluentValidation;

namespace JobBoard;

public class SignUpBusinessCommandValidator : AbstractValidator<SignUpBusinessCommand>
{
    public SignUpBusinessCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(50);
    }
}
