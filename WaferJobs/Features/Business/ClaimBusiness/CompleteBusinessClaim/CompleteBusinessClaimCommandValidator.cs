using FluentValidation;

namespace WaferJobs.Features.Business.ClaimBusiness.CompleteBusinessClaim;

public class CompleteBusinessClaimCommandValidator : AbstractValidator<CompleteBusinessClaimCommand>
{
    public CompleteBusinessClaimCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(100);
    }
}