using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ResetPassword;

public record ResetPasswordCommand(
    Guid UserId,
    string Token,
    string Password,
    string ConfirmPassword
) : IRequest<Result<Unit, Error>>;