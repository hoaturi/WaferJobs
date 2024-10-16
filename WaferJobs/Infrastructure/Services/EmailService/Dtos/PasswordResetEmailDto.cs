using WaferJobs.Domain.Auth;

namespace WaferJobs.Infrastructure.Services.EmailService.Dtos;

public record PasswordResetEmailDto(ApplicationUserEntity User, string Token);