using FluentValidation;

namespace WaferJobs.Features.Admin.Conference.VerifyConference;

public class VerifyConferenceCommandValidator : AbstractValidator<VerifyConferenceCommand>
{
    public VerifyConferenceCommandValidator()
    {
        RuleFor(x => x.ConferenceId).NotEmpty();
        RuleFor(x => x.IsApproved).NotEmpty();
    }
}