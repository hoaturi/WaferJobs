using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.ValidateBusinessCreationToken;

public class ValidateBusinessCreationTokenQueryHandler(AppDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<ValidateBusinessCreationTokenQuery, Result<ValidateBusinessCreationTokenResponse, Error>>
{
    public async Task<Result<ValidateBusinessCreationTokenResponse, Error>> Handle(
        ValidateBusinessCreationTokenQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var token = await dbContext.BusinessCreationTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.Token == request.Token && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .Select(x => new ValidateBusinessCreationTokenResponse(x.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null) return BusinessErrors.InvalidCreationToken;

        return token;
    }
}