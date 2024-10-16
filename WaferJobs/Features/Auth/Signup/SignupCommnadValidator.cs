using FluentValidation;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Auth.Signup;

public class SignupCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignupCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required")
            .Must(role => Enum.IsDefined(typeof(UserRoles), role))
            .WithMessage("Invalid role specified");

        RuleFor(x => x.Role)
            .Must(role => role is nameof(UserRoles.JobSeeker) or nameof(UserRoles.Business))
            .WithMessage("Invalid role specified");
    }
}