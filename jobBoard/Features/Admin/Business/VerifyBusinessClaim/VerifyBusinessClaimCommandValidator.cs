using FluentValidation;

namespace JobBoard.Features.Admin.Business.VerifyBusinessClaim;

public class VerifyBusinessClaimCommandValidator : AbstractValidator<VerifyBusinessClaimCommand>
{
    public VerifyBusinessClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
        RuleFor(x => x.Dto.Notes).MaximumLength(500);
        RuleFor(x => x.Dto.Action).Must(x =>
                x is nameof(ClaimAction.Approve) or nameof(ClaimAction.Reject))
            .WithMessage("Invalid action specified");
    }
}