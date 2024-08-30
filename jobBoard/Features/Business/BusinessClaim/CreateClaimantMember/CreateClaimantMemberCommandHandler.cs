using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.BusinessClaim.CreateClaimantMember;

public class CreateClaimantMemberCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<CreateClaimantMemberCommandHandler> logger
) : IRequestHandler<CreateClaimantMemberCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CreateClaimantMemberCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var claimRequest = await dbContext.BusinessClaimRequests
            .FirstOrDefaultAsync(x => x.ClaimantUserId == userId && x.IsVerified && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken) ?? throw new BusinessClaimRequestNotFoundException(userId);

        var business = await dbContext.Businesses.FirstOrDefaultAsync(x => x.Id == claimRequest.BusinessId,
            cancellationToken) ?? throw new BusinessNotFoundException(claimRequest.BusinessId);

        var newMember = new BusinessMemberEntity
        {
            BusinessId = claimRequest.BusinessId,
            UserId = userId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Title = command.Title,
            IsAdmin = !business.IsClaimed,
            JoinedAt = DateTime.UtcNow
        };

        business.IsClaimed = true;
        claimRequest.IsUsed = true;

        dbContext.BusinessMembers.Add(newMember);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} created a new member {MemberId} for business {BusinessId}",
            userId, newMember.Id, business.Id);

        return Unit.Value;
    }
}