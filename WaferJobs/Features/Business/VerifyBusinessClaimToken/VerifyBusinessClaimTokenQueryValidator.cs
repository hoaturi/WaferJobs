using FluentValidation;

namespace WaferJobs.Features.Business.VerifyBusinessClaimToken;

public class VerifyBusinessClaimTokenQueryValidator : AbstractValidator<VerifyBusinessClaimTokenQuery>
{
    public VerifyBusinessClaimTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}