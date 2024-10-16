using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ResetPassword;

public record ResetPasswordCommand(
    Guid UserId,
    string Token,
    string Password,
    string ConfirmPassword
) : IRequest<Result<Unit, Error>>;