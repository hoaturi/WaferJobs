using WaferJobs.Common;

namespace WaferJobs.Domain.Auth;

public class EmailChangeRequestEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string NewEmail { get; set; }
    public required string Pin { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int Attempts { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public ApplicationUserEntity User { get; set; } = null!;
}