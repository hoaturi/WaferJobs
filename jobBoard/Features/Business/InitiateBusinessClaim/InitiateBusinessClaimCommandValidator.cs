using FluentValidation;

namespace JobBoard.Features.Business.InitiateBusinessClaim;

public class InitiateBusinessClaimCommandValidator : AbstractValidator<InitiateBusinessClaimCommand>
{
    public InitiateBusinessClaimCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();
    }
}