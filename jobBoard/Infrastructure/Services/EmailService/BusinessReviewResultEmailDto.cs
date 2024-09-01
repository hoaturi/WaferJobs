namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessReviewResultEmailDto(
    Guid BusinessId,
    string RecipientEmail,
    string BusinessName,
    bool IsApproved);