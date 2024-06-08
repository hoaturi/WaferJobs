using FluentValidation;

namespace JobBoard.Features.Auth.CheckEmailAvailability;

public class CheckEmailAvailabilityQueryValidator : AbstractValidator<CheckEmailAvailabilityQuery>
{
    public CheckEmailAvailabilityQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address.");
    }
}