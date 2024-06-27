namespace JobBoard.Features.Auth.ResetPassword;

public record ResetPasswordRequestDto(string Password, string ConfirmPassword);