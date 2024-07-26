using FluentValidation;

namespace JobBoard.Features.JobAlert.SubscribeToJobAlert;

public class SubscribeToJobAlertCommandValidator : AbstractValidator<SubscribeToJobAlertCommand>
{
    public SubscribeToJobAlertCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Keyword).MaximumLength(50);
    }
}