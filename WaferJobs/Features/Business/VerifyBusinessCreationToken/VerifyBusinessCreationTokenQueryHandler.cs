using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.Business.VerifyBusinessCreationToken;

public class VerifyBusinessCreationTokenQueryHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<VerifyBusinessCreationTokenQueryHandler> logger)
    : IRequestHandler<VerifyBusinessCreationTokenQuery, Result<VerifyBusinessCreationTokenResponse, Error>>
{
    public async Task<Result<VerifyBusinessCreationTokenResponse, Error>> Handle(
        VerifyBusinessCreationTokenQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var token = await dbContext.BusinessCreationTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.Token == query.Token && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .Select(x => new VerifyBusinessCreationTokenResponse(x.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (token is not null) return token;

        logger.LogWarning("Business creation token {Token} for user {UserId} is not valid.", query.Token, userId);
        return BusinessErrors.InvalidCreationToken;
    }
}