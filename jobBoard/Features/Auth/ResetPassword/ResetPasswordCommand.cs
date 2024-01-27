using MediatR;

namespace JobBoard;

public record ResetPasswordCommand(
    Guid UserId,
    string Token,
    string Password,
    string ConfirmPassword
) : IRequest<Result<Unit, Error>>;
