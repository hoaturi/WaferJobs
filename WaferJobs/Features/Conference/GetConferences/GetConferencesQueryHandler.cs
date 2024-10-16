using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Conference.GetConferences;

public class
    GetConferencesQueryHandler(
        AppDbContext dbContext
    ) : IRequestHandler<GetConferencesQuery, Result<GetConferencesResponse, Error>>
{
    public async Task<Result<GetConferencesResponse, Error>> Handle(GetConferencesQuery query,
        CancellationToken cancellationToken)
    {
        var conferences = await dbContext.Conferences
            .AsNoTracking()
            .Where(c => c.IsPublished && c.EndDate > DateTime.UtcNow)
            .OrderBy(c => c.StartDate)
            .Select(c => new ConferenceItem(
                c.Title,
                c.Organiser,
                c.OrganiserEmail,
                c.Location,
                c.WebsiteUrl,
                c.LogoUrl,
                c.StartDate,
                c.EndDate
            ))
            .ToListAsync(cancellationToken);

        return new GetConferencesResponse(conferences);
    }
}