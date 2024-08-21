using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Business.GetClaimList;

public class GetClaimListQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetClaimListQuery, Result<GetClaimListResponse, Error>>
{
    public async Task<Result<GetClaimListResponse, Error>> Handle(GetClaimListQuery query,
        CancellationToken cancellationToken)
    {
        var claimQuery = dbContext.BusinessClaimAttempts.AsNoTracking();

        if (!string.IsNullOrEmpty(query.Status))
            if (Enum.TryParse<ClaimStatus>(query.Status, true, out var status))
                claimQuery = claimQuery.Where(x => x.Status == status);

        var claims = await claimQuery
            .Select(x => new ClaimListItem(
                x.Id,
                x.Business.Name,
                x.ClaimantEmail,
                x.ClaimantFirstName,
                x.ClaimantLastName,
                x.ClaimantTitle,
                x.ExpiresAt!.Value,
                x.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new GetClaimListResponse(claims);
    }
}