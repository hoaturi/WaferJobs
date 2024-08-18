namespace JobBoard.Infrastructure.Services.EmailService;

public record BusinessClaimEmailDto(
    string RecipientEmail,
    string FirstName,
    string BusinessName,
    string Token
);