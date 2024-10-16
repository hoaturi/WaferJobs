namespace WaferJobs.Infrastructure.Services.EmailService.Dtos;

public record BusinessCreationReviewEmailDto(string RecipientEmail, Guid BusinessId);