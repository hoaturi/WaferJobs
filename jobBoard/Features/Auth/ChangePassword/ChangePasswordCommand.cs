using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Result<Unit, Error>>;