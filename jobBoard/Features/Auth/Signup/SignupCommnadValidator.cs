using FluentValidation;

namespace JobBoard;

public class SignupCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignupCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .IsEnumName(typeof(RoleTypes))
            .WithMessage("Invalid role type");
    }
}
