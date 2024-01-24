using MediatR;

namespace JobBoard;

public record SignUpCommand(string Email, string Password) : IRequest<Result<Unit, Error>>;
