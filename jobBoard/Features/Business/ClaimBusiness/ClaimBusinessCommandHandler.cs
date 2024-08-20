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
    IBackgroundJobClient backgroundJobClient,
    AppDbContext dbContext,
    ILogger<ClaimBusinessCommandHandler> logger)
    : IRequestHandler<ClaimBusinessCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ClaimBusinessCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();
        var userEmail = currentUserService.GetUserEmail();

        await ValidateUserNotAlreadyMember(userId, cancellationToken);

        var business = await GetBusinessWithDetails(command.BusinessId, cancellationToken);

        ValidateBusinessClaimStatus(business);

        var userEmailDomain = userEmail.Split('@')[1];
        var isAutomaticApproval = userEmailDomain.Equals(business.Domain, StringComparison.OrdinalIgnoreCase);

        var newClaim = CreateNewClaimAttempt(business.Id, userId, userEmail, command, isAutomaticApproval);
        business.ClaimAttempts.Add(newClaim);

        var newMember = CreateNewBusinessMember(userId, command, isAutomaticApproval);
        business.Members.Add(newMember);

        await dbContext.SaveChangesAsync(cancellationToken);

        SendClaimResultEmail(isAutomaticApproval, userEmail, command.FirstName, business.Name);

        LogClaimResult(isAutomaticApproval, business.Id, userId);

        return Unit.Value;
    }

    private async Task ValidateUserNotAlreadyMember(Guid userId, CancellationToken cancellationToken)
    {
        if (await dbContext.BusinessMembers.AsNoTracking()
                .AnyAsync(m => m.UserId == userId && !m.IsDeleted, cancellationToken))
            throw new UserAlreadyMemberException(userId);
    }

    private async Task<BusinessEntity> GetBusinessWithDetails(Guid businessId, CancellationToken cancellationToken)
    {
        return await dbContext.Businesses
                   .Include(b => b.Members)
                   .Include(b => b.ClaimAttempts)
                   .FirstOrDefaultAsync(b => b.Id == businessId, cancellationToken)
               ?? throw new BusinessNotFoundException(businessId);
    }

    private static void ValidateBusinessClaimStatus(BusinessEntity business)
    {
        if (business.IsClaimed || business.ClaimAttempts.Any(ca => ca.Status == ClaimStatus.Approved))
            throw new BusinessAlreadyClaimedException(business.Id);

        var pendingClaimExists = business.ClaimAttempts.Any(ca =>
            ca.Status == ClaimStatus.Pending && ca.ExpiresAt > DateTime.UtcNow);

        if (pendingClaimExists) throw new BusinessClaimInProgressException(business.Id);
    }

    private static BusinessClaimAttemptEntity CreateNewClaimAttempt(Guid businessId, Guid userId, string userEmail,
        ClaimBusinessCommand command, bool isAutomaticApproval)
    {
        return new BusinessClaimAttemptEntity
        {
            BusinessId = businessId,
            ClaimantUserId = userId,
            ClaimantEmail = userEmail,
            ClaimantFirstName = command.FirstName,
            ClaimantLastName = command.LastName,
            ClaimantTitle = command.Title,
            Status = isAutomaticApproval ? ClaimStatus.Approved : ClaimStatus.Pending,
            ExpiresAt = isAutomaticApproval ? null : DateTime.UtcNow.AddDays(7)
        };
    }

    private static BusinessMemberEntity CreateNewBusinessMember(Guid userId, ClaimBusinessCommand command,
        bool isAutomaticApproval)
    {
        return new BusinessMemberEntity
        {
            UserId = userId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Title = command.Title,
            IsAdmin = true,
            JoinedAt = isAutomaticApproval ? DateTime.UtcNow : null,
            IsVerified = isAutomaticApproval
        };
    }

    private void SendClaimResultEmail(bool isAutomaticApproval, string userEmail, string firstName, string businessName)
    {
        if (!isAutomaticApproval)
            backgroundJobClient.Enqueue<EmailService>(x => x.SendBusinessClaimVerificationRequestAsync(
                new BusinessClaimVerificationRequestDto(userEmail, firstName, businessName)));
        else
            backgroundJobClient.Enqueue<EmailService>(x => x.SendBusinessClaimApprovalEmailAsync(
                new BusinessClaimVerificationResultDto(userEmail, firstName, businessName, ClaimStatus.Approved)));
    }

    private void LogClaimResult(bool isAutomaticApproval, Guid businessId, Guid userId)
    {
        if (!isAutomaticApproval)
            logger.LogWarning("Business claim for business {BusinessId} by user {UserId} requires verification.",
                businessId, userId);
        else
            logger.LogInformation("User {UserId} claimed business {BusinessId} successfully.", userId, businessId);
    }
}