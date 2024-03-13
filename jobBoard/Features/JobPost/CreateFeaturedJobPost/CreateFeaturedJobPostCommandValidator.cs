using FluentValidation;

namespace JobBoard;

public class CreateFeaturedJobPostCommandValidator : AbstractValidator<CreateFeaturedJobPostCommand>
{
    public CreateFeaturedJobPostCommandValidator()
    {
        RuleFor(jp => jp.CategoryId).NotEmpty();
        RuleFor(jp => jp.CountryId).NotEmpty();
        RuleFor(jp => jp.EmploymentTypeId).NotEmpty();
        RuleFor(jp => jp.Title).NotEmpty().MaximumLength(100);
        RuleFor(jp => jp.Description).NotEmpty().MaximumLength(10000);
        RuleFor(jp => jp.City).MaximumLength(50);
        RuleFor(jp => jp.ApplyUrl).MaximumLength(2000);
        RuleFor(jp => jp.CompanyName).NotEmpty().MaximumLength(50);
        RuleFor(jp => jp.IsRemote).NotEmpty();
        RuleFor(jp => jp.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(jp => jp.MaxSalary)
            .GreaterThanOrEqualTo(0)
            .Must((command, maxSalary) => maxSalary >= command.MinSalary)
            .WithMessage("MaxSalary must be greater than MinSalary");
        RuleFor(jp => jp.Currency).MaximumLength(3);
        RuleFor(jp => jp.Tags).Must(tags => tags?.Count <= 3).WithMessage("Tags must not exceed 3");
    }
}
