using MediatR;

namespace JobBoard;

public record ForgotPasswordCommand(string Email) : IRequest<Result<Unit, Error>>;
