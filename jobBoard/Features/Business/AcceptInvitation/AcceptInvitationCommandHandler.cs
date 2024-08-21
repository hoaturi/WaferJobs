using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.AcceptInvitation;

public class AcceptInvitationCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<AcceptInvitationCommandHandler> logger)
    : IRequestHandler<AcceptInvitationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();
        var userEmail = currentUserService.GetUserEmail();

        if (await MembershipExists(userId, cancellationToken))
        {
            logger.LogWarning("User {UserId} attempted to accept a business invitation but already has a membership",
                userId);
            return BusinessErrors.UserAlreadyBusinessMember;
        }

        var validInvitation = await GetValidInvitation(command.Token, cancellationToken);
        if (validInvitation is null)
        {
            logger.LogWarning("Invalid or expired invitation attempt for token: {Token}", command.Token);
            return BusinessErrors.InvalidOrExpiredInvitationToken;
        }

        if (validInvitation.InviteeEmail != userEmail)
        {
            logger.LogWarning(
                "User email {UserEmail} does not match the invitee email {InviteeEmail} for token: {Token}", userEmail,
                validInvitation.InviteeEmail, command.Token);
            return BusinessErrors.InvitationEmailMismatch;
        }

        AddBusinessMember(validInvitation, userId, command.Dto);
        await DeactivateUserInvitations(userEmail, validInvitation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} accepted business invitation for business {BusinessId}", userId,
            validInvitation.BusinessId);

        return Unit.Value;
    }

    private async Task<bool> MembershipExists(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.BusinessMembers
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    private async Task<BusinessMemberInvitationEntity?> GetValidInvitation(string token,
        CancellationToken cancellationToken)
    {
        return await dbContext.BusinessMemberInvitations
            .FirstOrDefaultAsync(
                bmi => bmi.Token == token && !bmi.IsAccepted && bmi.IsActive && bmi.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    private void AddBusinessMember(BusinessMemberInvitationEntity validInvitation, Guid userId, AcceptInvitationDto dto)
    {
        var membership = new BusinessMemberEntity
        {
            BusinessId = validInvitation.BusinessId,
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Title = dto.Title,
            JoinedAt = DateTime.UtcNow,
            IsVerified = true
        };

        dbContext.BusinessMembers.Add(membership);
    }

    private async Task DeactivateUserInvitations(string userEmail, BusinessMemberInvitationEntity validInvitation,
        CancellationToken cancellationToken)
    {
        var userInvitations = await dbContext.BusinessMemberInvitations
            .Where(bmi => bmi.InviteeEmail == userEmail && bmi.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var invitation in userInvitations)
        {
            invitation.IsActive = false;

            if (invitation.Id == validInvitation.Id)
            {
                invitation.IsAccepted = true;
                invitation.AcceptedAt = DateTime.UtcNow;
            }
        }
    }
}