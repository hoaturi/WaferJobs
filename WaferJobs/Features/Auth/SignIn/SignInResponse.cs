namespace WaferJobs.Features.Auth.SignIn;

public record SignInResponse(UserDto User, string AccessToken, string RefreshToken);

public record UserDto(Guid Id, string Email, IList<string> Roles, bool HasCompletedOnboarding);