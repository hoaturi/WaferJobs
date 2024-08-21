namespace JobBoard.Infrastructure.Services.EmailService;

public record PendingConferenceSubmissionReminderItem(
    string Title,
    DateTime CreatedAt
);

public record PendingConferenceSubmissionReminderDto(
    List<PendingConferenceSubmissionReminderItem> Conferences);