using FluentValidation;

namespace WaferJobs.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public class InitiateBusinessClaimCommandValidator : AbstractValidator<InitiateBusinessClaimCommand>
{
    public InitiateBusinessClaimCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();
    }
}