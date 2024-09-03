using JobBoard.Infrastructure.Services.EmailService.Dtos;

namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendEmailConfirmAsync(ConfirmEmailDto dto);
    Task SendPasswordResetAsync(PasswordResetEmailDto dto);
    Task SendEmailChangeVerificationAsync(EmailChangeVerificationEmailDto dto);
    Task SendJobAlertAsync(JobAlertEmailDto dto);
    Task SendBusinessClaimVerificationAsync(BusinessClaimVerificationEmailDto dto);
    Task SendBusinessCreationVerificationAsync(BusinessCreationVerificationEmailDto dto);
    Task SendBusinessCreationReviewAsync(BusinessCreationReviewEmailDto dto);
    Task SendBusinessReviewResultAsync(BusinessReviewResultEmailDto dto);
    Task SendBusinessMemberInvitationAsync(BusinessMemberInvitationEmailDto dto);

    Task SendConferenceSubmissionReviewAsync(
        ConferenceSubmissionReviewEmailDto dto);
}