using MediatR;

namespace JobBoard;

public record ResetPasswordCommand : IRequest<Result<Unit, Error>>
{
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
}
