namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record BusinessMemberInvitationEmailDto(
    string RecipientEmail,
    Guid BusinessId,
    string BusinessName,
    string InviterName,
    string Token,
    int Expiry
);