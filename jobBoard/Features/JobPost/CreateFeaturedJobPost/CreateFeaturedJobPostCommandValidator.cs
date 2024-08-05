using FluentValidation;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

public class CreateFeaturedJobPostCommandValidator<T> : AbstractValidator<T>
    where T : CreateFeaturedJobPostCommand
{
    public CreateFeaturedJobPostCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.EmploymentTypeId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.City).MaximumLength(50);
        RuleFor(x => x.ApplyUrl).MaximumLength(2000);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CompanyEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CompanyLogoUrl).MaximumLength(2000);
        RuleFor(x => x.CompanyWebsiteUrl).MaximumLength(2000);
        RuleFor(x => x.IsRemote).NotNull();
        RuleFor(x => x.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxSalary)
            .GreaterThanOrEqualTo(0)
            .Must(
                (command, maxSalary) =>
                {
                    if (command.MinSalary.HasValue && maxSalary.HasValue)
                        return maxSalary > command.MinSalary;
                    return true;
                }
            )
            .WithMessage("MaxSalary must be greater than MinSalary");
        RuleFor(x => x.Tags).Must(tags => tags?.Count <= 3).WithMessage("Tags must not exceed 3");
        RuleFor(x => x)
            .Must(x => !((x.MinSalary.HasValue || x.MaxSalary.HasValue) && !x.CurrencyId.HasValue))
            .WithMessage("CurrencyId must be provided when either MinSalary or MaxSalary is specified");
    }
}