using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Lookup.GetActiveJobCities;

public class GetActiveJobCitiesQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetActiveJobCitiesQuery, Result<GetActiveJobCitiesResponse, Error>>
{
    public async Task<Result<GetActiveJobCitiesResponse, Error>> Handle(GetActiveJobCitiesQuery request,
        CancellationToken cancellationToken)
    {
        var cities = await dbContext.JobPosts
            .AsNoTracking()
            .Where(j => j.City != null)
            .Select(j => j.City)
            .Distinct()
            .Select(c => new ActiveJobCityDto(c!.Id, c.Label, c.Slug))
            .ToListAsync(cancellationToken);

        return new GetActiveJobCitiesResponse(cities);
    }
}