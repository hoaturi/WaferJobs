using FluentValidation;
using JobBoard.Common.Constants;

namespace JobBoard.Features.Admin.Business.GetClaimList;

public class GetClaimListQueryValidator : AbstractValidator<GetClaimListQuery>
{
    public GetClaimListQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsEnumName(typeof(ClaimStatus), false)
            .When(x => x.Status is not null);
    }
}