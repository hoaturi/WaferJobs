namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessMemberInvitationDto(
    string RecipientEmail,
    string BusinessName,
    string InviterName,
    string Token
);