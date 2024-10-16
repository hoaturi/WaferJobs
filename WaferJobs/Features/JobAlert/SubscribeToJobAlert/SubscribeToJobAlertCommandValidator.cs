using FluentValidation;

namespace WaferJobs.Features.JobAlert.SubscribeToJobAlert;

public class SubscribeToJobAlertCommandValidator : AbstractValidator<SubscribeToJobAlertCommand>
{
    public SubscribeToJobAlertCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254).EmailAddress();
        RuleFor(x => x.Keyword)
            .MaximumLength(100);
    }
}