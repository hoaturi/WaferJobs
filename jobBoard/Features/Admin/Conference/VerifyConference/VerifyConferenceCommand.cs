using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Conference.VerifyConference;

public record VerifyConferenceCommand(
    int ConferenceId,
    bool IsApproved
) : IRequest<Result<Unit, Error>>;