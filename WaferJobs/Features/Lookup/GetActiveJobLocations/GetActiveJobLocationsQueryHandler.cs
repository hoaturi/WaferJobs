using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Lookup.GetActiveJobLocations;

public class GetActiveJobLocationsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetActiveJobLocationsQuery, Result<GetActiveJobLocationsResponse, Error>>
{
    public async Task<Result<GetActiveJobLocationsResponse, Error>> Handle(GetActiveJobLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var locations = await dbContext.JobPosts
            .AsNoTracking()
            .Where(j => j.IsPublished)
            .Select(j => new ActiveJobLocationDto(
                j.City != null ? $"{j.City.Label}, {j.Country.Label}" : j.Country.Label,
                j.City != null ? j.City.Slug : null,
                j.Country.Slug,
                j.CityId,
                j.CountryId
            ))
            .Distinct()
            .ToListAsync(cancellationToken);

        return new GetActiveJobLocationsResponse(locations);
    }
}