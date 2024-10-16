namespace WaferJobs.Features.Auth.ResetPassword;

public record ResetPasswordRequestDto(string Password, string ConfirmPassword);