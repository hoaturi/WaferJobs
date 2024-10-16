namespace WaferJobs.Infrastructure.Services.EmailService.Dtos;

public record BusinessMemberInvitationEmailDto(
    string RecipientEmail,
    Guid BusinessId,
    Guid InviterId,
    string BusinessName,
    string InviterName,
    string Token,
    int Expiry
);