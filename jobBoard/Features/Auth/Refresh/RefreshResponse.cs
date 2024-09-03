using JobBoard.Features.Auth.SignIn;

namespace JobBoard.Features.Auth.Refresh;

public record RefreshResponse(UserDto Dto, string AccessToken);