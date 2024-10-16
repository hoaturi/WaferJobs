using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Admin.Conference.VerifyConference;

public record VerifyConferenceCommand(
    int ConferenceId,
    bool IsApproved
) : IRequest<Result<Unit, Error>>;