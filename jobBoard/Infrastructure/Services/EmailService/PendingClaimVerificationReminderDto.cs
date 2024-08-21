namespace JobBoard.Infrastructure.Services.EmailService;

public record PendingClaimVerificationReminderItem(
    string BusinessName,
    DateTime ExpiresAt
);

public record PendingClaimVerificationReminderDto(
    List<PendingClaimVerificationReminderItem> Claims
);