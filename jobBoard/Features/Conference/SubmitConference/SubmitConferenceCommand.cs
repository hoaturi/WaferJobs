using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Conference.SubmitConference;

public record SubmitConferenceCommand(
    string ContactEmail,
    string ContactName,
    string Title,
    string Organiser,
    string OrganiserEmail,
    string Location,
    string WebsiteUrl,
    string StartDate,
    string EndDate
) : IRequest<Result<Unit, Error>>;