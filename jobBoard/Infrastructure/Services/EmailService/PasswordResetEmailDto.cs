using JobBoard.Domain.Auth;

namespace JobBoard.Infrastructure.Services.EmailService;

public record PasswordResetEmailDto(ApplicationUserEntity User, string Token);