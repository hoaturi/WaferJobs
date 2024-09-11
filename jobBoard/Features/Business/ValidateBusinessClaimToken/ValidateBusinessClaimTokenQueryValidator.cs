using FluentValidation;

namespace JobBoard.Features.Business.ValidateBusinessClaimToken;

public class ValidateBusinessClaimTokenQueryValidator : AbstractValidator<ValidateBusinessClaimTokenQuery>
{
    public ValidateBusinessClaimTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}