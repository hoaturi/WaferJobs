using WaferJobs.Infrastructure.BackgroundJobs.JobAlertEmailSenderJob;

namespace WaferJobs.Infrastructure.Services.EmailService.Dtos;

public record JobAlertEmailDto(
    string RecipientEmail,
    string? Keyword,
    string? Country,
    List<string>? Categories,
    List<string>? EmploymentTypes,
    string Token,
    string? FilterQuery,
    JobAlertEmailContentDto JobPosts
);