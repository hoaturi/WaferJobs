using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.VerifyBusinessClaimToken;

public class VerifyBusinessClaimTokenQueryHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<VerifyBusinessClaimTokenQueryHandler> logger
)
    : IRequestHandler<VerifyBusinessClaimTokenQuery, Result<VerifyBusinessClaimTokenResponse, Error>>
{
    public async Task<Result<VerifyBusinessClaimTokenResponse, Error>> Handle(VerifyBusinessClaimTokenQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var token = await dbContext.BusinessClaimTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.Token == query.Token && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .Select(x => new VerifyBusinessClaimTokenResponse(x.Business.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (token is not null) return token;

        logger.LogWarning("Business claim token {Token} for user {UserId} is not valid.", query.Token, userId);
        return BusinessErrors.InvalidClaimToken;
    }
}