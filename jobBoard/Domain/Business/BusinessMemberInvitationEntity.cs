using JobBoard.Common;

namespace JobBoard.Domain.Business;

public class BusinessMemberInvitationEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid InviterId { get; set; }
    public required string InviterName { get; set; }
    public required string InviteeEmail { get; set; }
    public required string Token { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public BusinessEntity Business { get; set; } = null!;
    public BusinessMemberEntity Inviter { get; set; } = null!;
}