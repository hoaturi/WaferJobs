using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.Business.ClaimBusiness.CompleteBusinessClaim;

public class CompleteBusinessClaimCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<CompleteBusinessClaimCommandHandler> logger
) : IRequestHandler<CompleteBusinessClaimCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CompleteBusinessClaimCommand command,
        CancellationToken cancellationToken)
    {
        var (userId, userEmail) = (currentUserService.GetUserId(), currentUserService.GetUserEmail());

        var token = await dbContext.BusinessClaimTokens
            .FirstOrDefaultAsync(
                x => x.Token == command.Token && x.UserId == userId && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (token is null)
        {
            logger.LogWarning("User {UserId} attempted to claim business with invalid token: {token}", userId, token);
            return BusinessErrors.InvalidClaimToken;
        }

        var business = await dbContext.Businesses.FirstOrDefaultAsync(x => x.Id == token.BusinessId,
            cancellationToken) ?? throw new BusinessNotFoundException(token.BusinessId);

        var userEmailDomain = userEmail.Split('@')[1];
        if (business.Domain != userEmailDomain) throw new EmailDomainMismatchException(userId, business.Id);

        token.IsUsed = true;
        token.UsedAt = DateTime.UtcNow;

        var newMember = new BusinessMembershipEntity
        {
            BusinessId = token.BusinessId,
            UserId = userId,
            FirstName = command.Dto.FirstName,
            LastName = command.Dto.LastName,
            Title = command.Dto.Title,
            IsAdmin = !business.IsClaimed,
            JoinedAt = DateTime.UtcNow
        };

        business.IsClaimed = true;
        token.IsUsed = true;

        dbContext.BusinessMemberships.Add(newMember);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Business {BusinessId} claimed by user {UserId}", business.Id, userId);

        return Unit.Value;
    }
}