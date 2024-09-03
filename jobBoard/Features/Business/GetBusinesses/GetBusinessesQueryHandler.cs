using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.GetBusinesses;

public class GetBusinessesQueryHandler(
    AppDbContext dbContext
) : IRequestHandler<GetBusinessesQuery, Result<GetBusinessesResponse, Error>>
{
    public async Task<Result<GetBusinessesResponse, Error>> Handle(GetBusinessesQuery query,
        CancellationToken cancellationToken)
    {
        var businessQuery = dbContext.Businesses.AsNoTracking()
            .Select(b => new BusinessListItem(b.Id, b.Name, b.Domain, b.LogoUrl));

        var businesses = await businessQuery.ToListAsync(cancellationToken);

        return new GetBusinessesResponse(businesses);
    }
}