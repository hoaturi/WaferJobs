using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Conference;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Admin.Conference.VerifyConference;

public class VerifyConferenceCommandHandler(
    AppDbContext dbContext,
    ILogger<VerifyConferenceCommand> logger)
    : IRequestHandler<VerifyConferenceCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(VerifyConferenceCommand command, CancellationToken cancellationToken)
    {
        var conference =
            await dbContext.Conferences.FirstOrDefaultAsync(
                c => c.Id == command.ConferenceId && !c.IsVerified && !c.IsPublished,
                cancellationToken) ?? throw new ConferenceNotFoundException(command.ConferenceId);

        conference.IsVerified = true;
        conference.IsPublished = command.IsApproved;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Completed verification for conference: {ConferenceId}. Result: {Result}",
            conference.Id,
            conference.IsVerified ? "Approved" : "Rejected");

        return Unit.Value;
    }
}