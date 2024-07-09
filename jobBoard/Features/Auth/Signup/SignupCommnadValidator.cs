using FluentValidation;
using JobBoard.Common.Constants;

namespace JobBoard.Features.Auth.Signup;

public class SignupCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignupCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(50);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(role => Enum.IsDefined(typeof(UserRoles), role))
            .WithMessage("Invalid role specified");
    }
}