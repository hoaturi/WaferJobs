using JobBoard.Infrastructure.Services.EmailService.Dtos;

namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendEmailConfirmAsync(ConfirmEmailDto dto);
    Task SendPasswordResetAsync(PasswordResetEmailDto dto);
    Task SendEmailChangeVerificationAsync(EmailChangeVerificationEmailDto dto);
    Task SendJobAlertAsync(JobAlertEmailDto dto);
    Task SendCompleteBusinessClaimAsync(CompleteBusinessClaimEmailDto dto);
    Task SendCompleteBusinessCreationAsync(CompleteBusinessCreationEmailDto dto);
    Task SendBusinessCreationReviewAsync(BusinessCreationReviewEmailDto dto);
    Task SendBusinessReviewResultAsync(BusinessReviewResultEmailDto dto);
    Task SendBusinessMemberInvitationAsync(BusinessMemberInvitationEmailDto dto);

    Task SendConferenceSubmissionReviewAsync(
        ConferenceSubmissionReviewEmailDto dto);
}