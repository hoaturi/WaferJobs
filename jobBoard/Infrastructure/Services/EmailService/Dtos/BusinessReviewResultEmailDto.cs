namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record BusinessReviewResultEmailDto(
    Guid BusinessId,
    string RecipientEmail,
    string BusinessName,
    bool IsApproved);