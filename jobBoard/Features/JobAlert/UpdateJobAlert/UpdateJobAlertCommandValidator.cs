using FluentValidation;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public class UpdateJobAlertCommandValidator : AbstractValidator<UpdateJobAlertCommand>
{
    public UpdateJobAlertCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Dto.Keyword).MaximumLength(100);
    }
}