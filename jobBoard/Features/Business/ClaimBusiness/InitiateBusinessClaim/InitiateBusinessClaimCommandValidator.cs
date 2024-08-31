using FluentValidation;

namespace JobBoard.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public class InitiateBusinessClaimCommandValidator : AbstractValidator<InitiateBusinessClaimCommand>
{
    public InitiateBusinessClaimCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();
    }
}