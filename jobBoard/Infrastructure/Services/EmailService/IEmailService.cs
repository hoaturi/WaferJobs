namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendEmailConfirmAsync(ConfirmEmailDto dto);
    Task SendPasswordResetAsync(PasswordResetEmailDto dto);
    Task SendEmailChangeVerificationAsync(EmailChangeVerificationDto dto);
    Task SendJobAlertAsync(JobAlertEmailDto dto);

    Task SendBusinessMemberInvitationAsync(
        BusinessMemberInvitationDto dto);

    Task SendClaimVerificationAsync(ClaimVerificationDto dto);

    Task SendConferenceSubmissionReviewAsync(
        ConferenceSubmissionReviewDto dto);
}