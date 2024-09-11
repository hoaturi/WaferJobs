using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.ValidateBusinessClaimToken;

public class ValidateBusinessClaimTokenQueryHandler(AppDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<ValidateBusinessClaimTokenQuery, Result<ValidateBusinessClaimTokenResponse, Error>>
{
    public async Task<Result<ValidateBusinessClaimTokenResponse, Error>> Handle(ValidateBusinessClaimTokenQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var token = await dbContext.BusinessClaimTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.Token == query.Token && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .Select(x => new ValidateBusinessClaimTokenResponse(x.Business.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null) return BusinessErrors.InvalidClaimToken;

        return token;
    }
}