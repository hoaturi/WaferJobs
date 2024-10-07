using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Business.GetPendingBusinesses;

public class GetPendingBusinessesQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetPendingBusinessesQuery, Result<GetPendingBusinessesQueryResponse, Error>>
{
    public async Task<Result<GetPendingBusinessesQueryResponse, Error>> Handle(GetPendingBusinessesQuery query,
        CancellationToken cancellationToken)
    {
        var businesses = await dbContext.Businesses
            .AsNoTracking()
            .Where(b => !b.IsActive)
            .Select(b => new PendingBusinessDto(
                b.Id,
                b.Name,
                b.WebsiteUrl,
                b.Domain,
                b.Memberships.FirstOrDefault(m => m.IsAdmin)!.User.Email!,
                $"{b.Memberships.FirstOrDefault(m => m.IsAdmin)!.FirstName} {b.Memberships.FirstOrDefault(m => m.IsAdmin)!.LastName}",
                b.Memberships.FirstOrDefault(m => m.IsAdmin)!.Title,
                b.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new GetPendingBusinessesQueryResponse(businesses);
    }
}