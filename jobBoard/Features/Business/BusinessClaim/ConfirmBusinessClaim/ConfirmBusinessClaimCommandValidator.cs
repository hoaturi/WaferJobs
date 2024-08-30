using FluentValidation;
using JobBoard.Common.Constants;

namespace JobBoard.Features.Business.BusinessClaim.ConfirmBusinessClaim;

public class ConfirmBusinessClaimCommandValidator : AbstractValidator<ConfirmBusinessClaimCommand>
{
    public ConfirmBusinessClaimCommandValidator()
    {
        RuleFor(x => x.Pin).NotEmpty()
            .InclusiveBetween(PinConstants.MinValue, PinConstants.MaxValue);
    }
}