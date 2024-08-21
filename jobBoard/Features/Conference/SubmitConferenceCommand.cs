using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Conference;

public record SubmitConferenceCommand(
    string ContactEmail,
    string ContactName,
    string Title,
    string OrganizerName,
    string OrganizerEmail,
    string Location,
    string WebsiteUrl,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<Unit, Error>>;