using WaferJobs.Common;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Domain.Business;

public class BusinessClaimTokenEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime UsedAt { get; set; }
    public BusinessEntity Business { get; set; } = null!;
    public ApplicationUserEntity User { get; set; } = null!;
}