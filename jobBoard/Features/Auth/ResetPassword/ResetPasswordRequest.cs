namespace JobBoard.Features.Auth.ResetPassword;

public record ResetPasswordRequest(string Password, string ConfirmPassword);