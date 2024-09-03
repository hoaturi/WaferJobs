using FluentValidation;

namespace JobBoard.Features.Business.InviteBusinessMember.SendInvitation;

public class SendInvitationCommandValidator : AbstractValidator<SendInvitationCommand>
{
    public SendInvitationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}