using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Lookup.GetActiveJobCities;

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