using JobBoard.Features.Auth.SignIn;

namespace JobBoard.Features.Auth.Refresh;

public record RefreshResponse(UserDto User, string AccessToken);