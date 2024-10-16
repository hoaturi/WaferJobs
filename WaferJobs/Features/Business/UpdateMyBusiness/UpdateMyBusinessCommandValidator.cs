using FluentValidation;

namespace WaferJobs.Features.Business.UpdateMyBusiness;

public class UpdateMyBusinessCommandValidator : AbstractValidator<UpdateMyBusinessCommand>
{
    public UpdateMyBusinessCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Location)
            .MaximumLength(100);

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(2048);

        RuleFor(x => x.TwitterUrl)
            .MaximumLength(2048);

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(2048);

        RuleFor(x => x.BusinessSizeId)
            .GreaterThanOrEqualTo(1);
    }
}