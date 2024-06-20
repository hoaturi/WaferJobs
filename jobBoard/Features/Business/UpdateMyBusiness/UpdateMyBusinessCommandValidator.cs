using FluentValidation;

namespace JobBoard.Features.Business.UpdateMyBusiness;

public class UpdateMyBusinessCommandValidator : AbstractValidator<UpdateMyBusinessCommand>
{
    public UpdateMyBusinessCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(5000);

        RuleFor(x => x.Location)
            .MaximumLength(50);

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(500);

        RuleFor(x => x.TwitterUrl)
            .MaximumLength(500);

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500);

        RuleFor(x => x.BusinessSizeId)
            .GreaterThanOrEqualTo(1);
    }
}