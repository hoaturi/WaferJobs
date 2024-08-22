using JobBoard.Common.Models;
using JobBoard.Domain.Conference;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CachingServices.ConferenceService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Conference.VerifyConference;

public class VerifyConferenceCommandHandler(
    AppDbContext dbContext,
    IConferenceService conferenceService,
    ILogger<VerifyConferenceCommand> logger)
    : IRequestHandler<VerifyConferenceCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(VerifyConferenceCommand command, CancellationToken cancellationToken)
    {
        var conference =
            await dbContext.Conferences.FirstOrDefaultAsync(c => c.Id == command.ConferenceId, cancellationToken);

        if (conference is null)
            throw new ConferenceNotFoundException(command.ConferenceId);

        conference.IsVerified = true;
        conference.IsPublished = command.IsApproved;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (command.IsApproved) await conferenceService.RefreshConferencesCacheAsync(cancellationToken);

        logger.LogInformation("Conference with ID: {ConferenceId} has been verified", conference.Id);

        return Unit.Value;
    }
}