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
                b.WebsiteUrl ?? string.Empty,
                b.Domain ?? string.Empty,
                b.Members.FirstOrDefault(m => m.IsAdmin)!.User.Email!,
                $"{b.Members.FirstOrDefault(m => m.IsAdmin)!.FirstName} {b.Members.FirstOrDefault(m => m.IsAdmin)!.LastName}",
                b.Members.FirstOrDefault(m => m.IsAdmin)!.Title,
                b.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new GetPendingBusinessesQueryResponse(businesses);
    }
}