using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class SendGridOptions
{
    public const string Key = "SendGrid";

    [Required] public required string ApiKey { get; init; }
    [Required] public required string EmailChangeConfirmationTemplateId { get; init; }
    [Required] public required string JobAlertTemplateId { get; init; }
    [Required] public required string PasswordResetTemplateId { get; init; }
    [Required] public required string ConfirmEmailTemplateId { get; init; }
    [Required] public required string BusinessClaimVerificationTemplateId { get; init; }
    [Required] public required string BusinessCreationVerificationTemplateId { get; init; }
    [Required] public required string BusinessCreationReviewTemplateId { get; init; }
    [Required] public required string BusinessReviewApprovedTemplateId { get; init; }
    [Required] public required string BusinessReviewRejectedTemplateId { get; init; }
    [Required] public required string BusinessMemberInvitationTemplateId { get; init; }
    [Required] public required string ConferenceSubmissionReviewTemplateId { get; init; }
    [Required] public required string PendingConferenceSubmissionReminderTemplateId { get; init; }
}