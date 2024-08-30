using FluentValidation;

namespace JobBoard.Features.Business.BusinessClaim.CreateClaimantMember;

public class CreateClaimantMemberCommandValidator : AbstractValidator<CreateClaimantMemberCommand>
{
    public CreateClaimantMemberCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
    }
}