using FluentValidation;

namespace JobBoard;

public class UpdateMyBusinessCommandValidator : AbstractValidator<UpdateMyBusinessCommand>
{
    public UpdateMyBusinessCommandValidator()
    {
        RuleFor(b => b.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(5000);
        RuleFor(b => b.Location).MaximumLength(50);
        RuleFor(b => b.Url).MaximumLength(500);
        RuleFor(b => b.TwitterUrl).MaximumLength(500);
        RuleFor(b => b.LinkedInUrl).MaximumLength(500);
        RuleFor(b => b.BusinessSizeId).GreaterThanOrEqualTo(1);
    }
}
