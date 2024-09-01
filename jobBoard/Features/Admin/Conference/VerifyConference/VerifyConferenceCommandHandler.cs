using JobBoard.Common.Models;
using JobBoard.Domain.Conference;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Conference.VerifyConference;

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

        logger.LogInformation("Conference with ID: {ConferenceId} has been verified", conference.Id);

        return Unit.Value;
    }
}