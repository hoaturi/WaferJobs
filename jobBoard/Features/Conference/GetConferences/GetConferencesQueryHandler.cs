using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Conference.GetConferences;

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
            .Select(c => new ConferenceDto(
                c.Title,
                c.Organiser,
                c.OrganiserEmail,
                c.Location,
                c.WebsiteUrl,
                c.StartDate,
                c.EndDate
            ))
            .ToListAsync(cancellationToken);

        return new GetConferencesResponse(conferences);
    }
}