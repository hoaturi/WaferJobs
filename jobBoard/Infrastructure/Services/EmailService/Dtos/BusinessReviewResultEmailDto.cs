namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record BusinessReviewResultEmailDto(
    Guid BusinessId,
    Guid UserId,
    string RecipientEmail,
    string BusinessName,
    bool IsApproved);