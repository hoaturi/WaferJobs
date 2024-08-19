using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.ClaimBusiness;

public class ClaimBusinessCommandHandler(
    ICurrentUserService currentUserService,
    IEmailService emailService,
    IBackgroundJobClient backgroundJobClient,
    AppDbContext dbContext,
    ILogger<ClaimBusinessCommandHandler> logger
) : IRequestHandler<ClaimBusinessCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ClaimBusinessCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var isAlreadyBusinessMember =
            await dbContext.BusinessMembers.AsNoTracking()
                .AnyAsync(m => m.UserId == userId && !m.IsDeleted, cancellationToken);
        if (isAlreadyBusinessMember) throw new UserAlreadyMemberException(userId);

        var business = await dbContext.Businesses
            .Include(b => b.Members)
            .Include(b => b.ClaimAttempt)
            .FirstOrDefaultAsync(b => b.Id == command.BusinessId, cancellationToken);

        if (business is null) throw new BusinessNotFoundException(command.BusinessId);
        if (business.IsClaimed || business.ClaimAttempt?.Status is ClaimStatus.Approved)
            throw new BusinessAlreadyClaimedException(command.BusinessId);

        if (business.ClaimAttempt?.Status is ClaimStatus.Pending)
            throw new BusinessClaimInProgressException(command.BusinessId);

        var userEmail = currentUserService.GetUserEmail();
        var userEmailDomain = userEmail.Split('@')[1];

        var doEmailDomainsMatch = userEmailDomain.Equals(business.Domain, StringComparison.OrdinalIgnoreCase);

        var newClaim = new BusinessClaimAttemptEntity
        {
            BusinessId = business.Id,
            ClaimantUserId = userId,
            ClaimantEmail = userEmail,
            ClaimantFirstName = command.FirstName,
            ClaimantLastName = command.LastName,
            ClaimantTitle = command.Title,
            Status = doEmailDomainsMatch ? ClaimStatus.Approved : ClaimStatus.Pending
        };

        var newMember = new BusinessMemberEntity
        {
            UserId = userId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Title = command.Title,
            IsAdmin = true,
            JoinedAt = doEmailDomainsMatch ? DateTime.UtcNow : default,
            IsVerified = doEmailDomainsMatch
        };

        business.Members.Add(newMember);
        business.ClaimAttempt = newClaim;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (!doEmailDomainsMatch)
        {
            backgroundJobClient.Enqueue(() => emailService.SendBusinessClaimVerificationRequestAsync(
                new BusinessClaimVerificationRequestDto(
                    userEmail,
                    command.FirstName,
                    business.Name
                )
            ));

            logger.LogWarning(
                "Business claim for business {BusinessId} by user {UserId} requires verification.",
                business.Id, userId);
        }
        else
        {
            backgroundJobClient.Enqueue(() => emailService.SendBusinessClaimApprovalEmailAsync(
                new BusinessClaimVerificationResultDto(
                    userEmail,
                    command.FirstName,
                    business.Name,
                    ClaimStatus.Approved
                )
            ));

            logger.LogInformation(
                "user {UserId} claimed business {BusinessId} successfully.", userId, business.Id);
        }

        return Unit.Value;
    }
}