using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Result<Unit, Error>>;