using FluentValidation;

namespace WaferJobs.Features.Business.CreateBusiness.InitiateBusinessCreation;

public class InitiateBusinessCreationCommandValidator : AbstractValidator<InitiateBusinessCreationCommand>
{
    public InitiateBusinessCreationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .MaximumLength(2048);
    }
}