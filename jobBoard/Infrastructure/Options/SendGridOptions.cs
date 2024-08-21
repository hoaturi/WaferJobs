using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class SendGridOptions
{
    public const string Key = "SendGrid";

    [Required] public required string ApiKey { get; init; }
    [Required] public required string JobAlertTemplateId { get; init; }
    [Required] public required string PasswordResetTemplateId { get; init; }
    [Required] public required string ConfirmEmailTemplateId { get; init; }
    [Required] public required string BusinessClaimVerificationRequestTemplateId { get; init; }
    [Required] public required string BusinessClaimApprovedTemplateId { get; init; }
    [Required] public required string BusinessClaimRejectedTemplateId { get; init; }
    [Required] public required string BusinessMemberInvitationTemplateId { get; init; }
    [Required] public required string BusinessClaimVerificationReminderTemplateId { get; init; }
}