using FluentValidation;

namespace JobBoard.Features.Conference.SubmitConference;

public class SubmitConferenceCommandValidator : AbstractValidator<SubmitConferenceCommand>
{
    public SubmitConferenceCommandValidator()
    {
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Organiser).NotEmpty().MaximumLength(150);
        RuleFor(x => x.OrganiserEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(150);
        RuleFor(x => x.WebsiteUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}