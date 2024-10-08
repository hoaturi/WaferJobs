namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record ConferenceSubmissionReviewEmailDto(
    int ConferenceId,
    string Title
);