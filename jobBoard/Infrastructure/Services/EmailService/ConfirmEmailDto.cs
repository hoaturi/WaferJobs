using JobBoard.Domain.Auth;

namespace JobBoard.Infrastructure.Services.EmailService;

public record ConfirmEmailDto(ApplicationUserEntity User, string Token);