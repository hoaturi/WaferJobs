using WaferJobs.Common;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Domain.Business;

public class BusinessCreationTokenEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Domain { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime UsedAt { get; set; }
    public ApplicationUserEntity User { get; set; } = null!;
}