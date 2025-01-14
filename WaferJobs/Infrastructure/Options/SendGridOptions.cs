﻿using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class SendGridOptions
{
    public const string Key = "SendGrid";

    [Required] public required string ApiKey { get; init; }
    [Required] public required string EmailChangeConfirmationTemplateId { get; init; }
    [Required] public required string JobAlertTemplateId { get; init; }
    [Required] public required string PasswordResetTemplateId { get; init; }
    [Required] public required string ConfirmEmailTemplateId { get; init; }
    [Required] public required string CompleteBusinessClaimTemplateId { get; init; }
    [Required] public required string CompleteBusinessCreationTemplateId { get; init; }
    [Required] public required string BusinessCreationReviewTemplateId { get; init; }
    [Required] public required string BusinessReviewApprovedTemplateId { get; init; }
    [Required] public required string BusinessReviewRejectedTemplateId { get; init; }
    [Required] public required string BusinessMemberInvitationTemplateId { get; init; }
    [Required] public required string ConferenceSubmissionReviewTemplateId { get; init; }
    [Required] public required string PendingConferenceSubmissionReminderTemplateId { get; init; }
}