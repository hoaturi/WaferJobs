namespace JobBoard;

public record SignInResponse(UserResponse User, string AccessToken, string RefreshToken);

public record UserResponse(Guid Id, string Email, IList<string> Roles);
