using JobBoard.Domain.Auth;

namespace JobBoard.Infrastructure.Services.EmailService.Dtos;

public record PasswordResetEmailDto(ApplicationUserEntity User, string Token);