namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record BusinessCreationReviewEmailDto(string RecipientEmail, Guid BusinessId);