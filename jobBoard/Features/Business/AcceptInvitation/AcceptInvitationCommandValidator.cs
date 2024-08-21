using FluentValidation;

namespace JobBoard.Features.Business.AcceptInvitation;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Dto.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Dto.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}