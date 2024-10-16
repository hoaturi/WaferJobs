using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Conference.SubmitConference;

public record SubmitConferenceCommand(
    string ContactEmail,
    string ContactName,
    string Title,
    string Organiser,
    string OrganiserEmail,
    string Location,
    string WebsiteUrl,
    string? LogoUrl,
    string StartDate,
    string EndDate
) : IRequest<Result<Unit, Error>>;