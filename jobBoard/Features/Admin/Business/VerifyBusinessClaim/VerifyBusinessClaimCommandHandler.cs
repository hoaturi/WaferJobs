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

namespace JobBoard.Features.Admin.Business.VerifyBusinessClaim;

public class VerifyBusinessClaimCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    IEmailService emailService,
    ILogger<VerifyBusinessClaimCommandHandler> logger)
    : IRequestHandler<VerifyBusinessClaimCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(VerifyBusinessClaimCommand command,
        CancellationToken cancellationToken)
    {
        var pendingClaim = await GetPendingClaimWithDetails(command.ClaimId, cancellationToken);

        if (pendingClaim.ExpiresAt < DateTime.UtcNow)
            return BusinessErrors.BusinessClaimExpired;

        ValidateClaimStatus(pendingClaim);

        var claimantMember = GetClaimantMember(pendingClaim);

        var verifierId = currentUserService.GetUserId();

        ProcessClaimAction(command.Dto.Action, pendingClaim, claimantMember, verifierId);

        UpdateClaimDetails(pendingClaim, verifierId, command.Dto.Notes);

        await dbContext.SaveChangesAsync(cancellationToken);

        SendVerificationResultEmail(pendingClaim);

        return Unit.Value;
    }

    private async Task<BusinessClaimAttemptEntity> GetPendingClaimWithDetails(Guid claimId,
        CancellationToken cancellationToken)
    {
        return await dbContext.BusinessClaimAttempts
            .Where(bc => bc.Id == claimId)
            .Include(bc => bc.Business)
            .Include(bc => bc.Business.Members)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new BusinessClaimantNotFoundException(claimId);
    }

    private static void ValidateClaimStatus(BusinessClaimAttemptEntity pendingClaim)
    {
        if (pendingClaim.Business.IsClaimed)
            throw new BusinessAlreadyClaimedException(pendingClaim.Business.Id);

        if (pendingClaim.Status != ClaimStatus.Pending)
            throw new BusinessClaimAlreadyVerifiedException(pendingClaim.Id);
    }

    private static BusinessMemberEntity GetClaimantMember(BusinessClaimAttemptEntity pendingClaim)
    {
        return pendingClaim.Business.Members
                   .FirstOrDefault(m => m.UserId == pendingClaim.ClaimantUserId && !m.IsDeleted)
               ?? throw new BusinessClaimantNotFoundException(pendingClaim.ClaimantUserId);
    }

    private void ProcessClaimAction(string action, BusinessClaimAttemptEntity pendingClaim,
        BusinessMemberEntity claimantMember, Guid verifierId)
    {
        switch (action)
        {
            case nameof(ClaimAction.Approve):
                ApproveBusinessClaim(pendingClaim, claimantMember, verifierId);
                break;
            case nameof(ClaimAction.Reject):
                RejectBusinessClaim(pendingClaim, claimantMember, verifierId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, "Invalid claim action");
        }
    }

    private void ApproveBusinessClaim(BusinessClaimAttemptEntity pendingClaim, BusinessMemberEntity claimantMember,
        Guid verifierId)
    {
        pendingClaim.Status = ClaimStatus.Approved;
        pendingClaim.Business.IsClaimed = true;
        claimantMember.IsAdmin = true;
        claimantMember.JoinedAt = DateTime.UtcNow;
        claimantMember.IsVerified = true;

        logger.LogInformation(
            "Business claim {ClaimId} for business {BusinessId} approved by verifier {VerifierId}. Claimant {ClaimantId} granted admin rights.",
            pendingClaim.Id, pendingClaim.Business.Id, verifierId, claimantMember.UserId);
    }

    private void RejectBusinessClaim(BusinessClaimAttemptEntity pendingClaim, BusinessMemberEntity claimantMember,
        Guid verifierId)
    {
        pendingClaim.Status = ClaimStatus.Rejected;
        claimantMember.IsDeleted = true;

        logger.LogInformation(
            "Business claim {ClaimId} for business {BusinessId} rejected by verifier {VerifierId}. Claimant {ClaimantId} removed.",
            pendingClaim.Id, pendingClaim.Business.Id, verifierId, claimantMember.UserId);
    }

    private static void UpdateClaimDetails(BusinessClaimAttemptEntity pendingClaim, Guid verifierId, string? notes)
    {
        pendingClaim.VerifierId = verifierId;
        pendingClaim.VerificationDate = DateTime.UtcNow;
        pendingClaim.Notes = notes;
    }

    private void SendVerificationResultEmail(BusinessClaimAttemptEntity pendingClaim)
    {
        backgroundJobClient.Enqueue(() => emailService.SendBusinessClaimVerificationResultAsync(
            new BusinessClaimVerificationResultDto(
                pendingClaim.ClaimantEmail,
                pendingClaim.ClaimantFirstName,
                pendingClaim.Business.Name,
                pendingClaim.Status
            )));
    }
}