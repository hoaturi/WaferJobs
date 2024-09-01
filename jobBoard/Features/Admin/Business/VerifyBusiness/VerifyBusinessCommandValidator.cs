using FluentValidation;

namespace JobBoard.Features.Admin.Business.VerifyBusiness;

public class VerifyBusinessCommandValidator : AbstractValidator<VerifyBusinessCommand>
{
    public VerifyBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId).NotEmpty();
        RuleFor(x => x.IsApproved).NotEmpty();
    }
}