using JobBoard.Domain.Auth;

namespace JobBoard.Infrastructure.Services.EmailService;

public record PasswordResetDto(ApplicationUserEntity UserEntity, string Token);