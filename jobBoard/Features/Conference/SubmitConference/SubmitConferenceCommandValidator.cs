using FluentValidation;

namespace JobBoard.Features.Conference.SubmitConference;

public class SubmitConferenceCommandValidator : AbstractValidator<SubmitConferenceCommand>
{
    public SubmitConferenceCommandValidator()
    {
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.OrganizerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.OrganizerEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WebsiteUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
    }
}