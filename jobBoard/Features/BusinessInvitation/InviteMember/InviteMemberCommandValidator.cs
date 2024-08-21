using FluentValidation;

namespace JobBoard.Features.BusinessInvitation.InviteMember;

public class InviteMemberCommandValidator : AbstractValidator<InviteMemberCommand>
{
    public InviteMemberCommandValidator()
    {
        RuleFor(x => x.InviteeEmail)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();
    }
}