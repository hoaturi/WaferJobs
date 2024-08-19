using JobBoard.Common;
using JobBoard.Common.Constants;
using JobBoard.Domain.Auth;

namespace JobBoard.Domain.Business;

public class BusinessClaimAttemptEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid ClaimantUserId { get; set; }
    public Guid? VerifierId { get; set; }
    public required string ClaimantEmail { get; set; }
    public required string ClaimantFirstName { get; set; }
    public required string ClaimantLastName { get; set; }
    public required string ClaimantTitle { get; set; }
    public string? Notes { get; set; }
    public ClaimStatus Status { get; set; }
    public DateTime? VerificationDate { get; set; }
    public BusinessEntity Business { get; set; } = null!;
    public ApplicationUserEntity ClaimantUser { get; set; } = null!;
    public ApplicationUserEntity? Verifier { get; set; }
}