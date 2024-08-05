using FluentValidation;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public class UpdateMyJobPostCommandValidator : AbstractValidator<UpdateMyJobPostCommand>
{
    public UpdateMyJobPostCommandValidator()
    {
        RuleFor(x => x.Dto.CategoryId).NotEmpty();
        RuleFor(x => x.Dto.CountryId).NotEmpty();
        RuleFor(x => x.Dto.EmploymentTypeId).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.Dto.City).MaximumLength(50);
        RuleFor(x => x.Dto.ApplyUrl).MaximumLength(2048);
        RuleFor(x => x.Dto.CompanyName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.CompanyEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Dto.CompanyLogoUrl).MaximumLength(2048);
        RuleFor(x => x.Dto.CompanyWebsiteUrl).MaximumLength(2048);
        RuleFor(x => x.Dto.IsRemote).NotNull();
        RuleFor(x => x.Dto.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dto.MaxSalary)
            .GreaterThanOrEqualTo(0)
            .Must(
                (command, maxSalary) =>
                {
                    if (command.Dto.MinSalary.HasValue && maxSalary.HasValue)
                        return maxSalary > command.Dto.MinSalary;
                    return true;
                }
            )
            .WithMessage("MaxSalary must be greater than MinSalary");

        RuleFor(x => x)
            .Must(x => !((x.Dto.MinSalary.HasValue || x.Dto.MaxSalary.HasValue) && !x.Dto.CurrencyId.HasValue))
            .WithMessage("CurrencyId must be provided when either MinSalary or MaxSalary is specified");

        RuleFor(x => x.Dto.Tags).Must(tags => tags?.Count <= 3).WithMessage("Tags must not exceed 3");
    }
}