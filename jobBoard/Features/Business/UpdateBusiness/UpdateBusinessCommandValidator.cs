using FluentValidation;

namespace JobBoard;

public class UpdateBusinessCommandValidator : AbstractValidator<UpdateBusinessCommand>
{
    public UpdateBusinessCommandValidator()
    {
        RuleFor(b => b.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(5000);
        RuleFor(b => b.Location).MaximumLength(50);
        RuleFor(b => b.Size)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(BusinessSize.BusinessSizes.Count);
        RuleFor(b => b.Url).MaximumLength(500);
        RuleFor(b => b.TwitterUrl).MaximumLength(500);
        RuleFor(b => b.LinkedInUrl).MaximumLength(500);
    }
}
