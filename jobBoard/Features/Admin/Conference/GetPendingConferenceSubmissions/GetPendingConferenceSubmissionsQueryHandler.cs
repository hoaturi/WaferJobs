using System.Data.Entity;
using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;

namespace JobBoard.Features.Admin.Conference.GetPendingConferenceSubmissions;

public class GetPendingConferenceSubmissionsQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetPendingConferenceSubmissionsQuery, Result<GetPendingConferencesResponse, Error>>
{
    public async Task<Result<GetPendingConferencesResponse, Error>> Handle(GetPendingConferenceSubmissionsQuery query,
        CancellationToken cancellationToken)
    {
        var pendingSubmissions = await dbContext.Conferences
            .AsNoTracking()
            .Where(c => !c.IsVerified)
            .Select(c => new PendingConference(
                c.Id,
                c.ContactEmail,
                c.ContactName,
                c.Title,
                c.OrganizerName,
                c.OrganizerEmail,
                c.Location,
                c.WebsiteUrl,
                c.StartDate,
                c.EndDate
            ))
            .ToListAsync(cancellationToken);

        return new GetPendingConferencesResponse(pendingSubmissions);
    }
}