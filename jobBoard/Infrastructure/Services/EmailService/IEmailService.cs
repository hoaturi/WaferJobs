namespace JobBoard.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task SendEmailConfirmAsync(ConfirmEmailDto dto);
    Task SendPasswordResetAsync(PasswordResetEmailDto dto);
    Task SendJobAlertAsync(JobAlertEmailDto dto);

    Task SendBusinessClaimVerificationRequestAsync(
        BusinessClaimVerificationRequestDto dto);

    Task SendBusinessClaimVerificationResultAsync(
        BusinessClaimVerificationResultDto dto);

    Task SendBusinessClaimApprovalEmailAsync(
        BusinessClaimVerificationResultDto dto);
}