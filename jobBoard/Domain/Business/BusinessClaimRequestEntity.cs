using JobBoard.Common;
using JobBoard.Domain.Auth;

namespace JobBoard.Domain.Business;

public class BusinessClaimRequestEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid ClaimantUserId { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int Pin { get; set; }
    public BusinessEntity Business { get; set; } = null!;
    public ApplicationUserEntity ClaimantUser { get; set; } = null!;
}