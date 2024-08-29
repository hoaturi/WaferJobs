using JobBoard.Common;

namespace JobBoard.Domain.Auth;

public class EmailChangeRequestEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public required string NewEmail { get; set; }
    public int Pin { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public ApplicationUserEntity User { get; set; } = null!;
}