using FluentValidation;

namespace WaferJobs.Features.Lookup.ValidatePublicDomain;

public class ValidatePublicDomainCommandValidator : AbstractValidator<ValidatePublicDomainCommand>
{
    public ValidatePublicDomainCommandValidator()
    {
        RuleFor(x => x.Domain)
            .NotEmpty()
            .WithMessage("Domain is required");
    }
}