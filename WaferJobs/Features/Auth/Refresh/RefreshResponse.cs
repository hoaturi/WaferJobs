using WaferJobs.Features.Auth.SignIn;

namespace WaferJobs.Features.Auth.Refresh;

public record RefreshResponse(UserDto User, string AccessToken);