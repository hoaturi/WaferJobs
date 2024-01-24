using MediatR;

namespace JobBoard;

public record SignOutCommand(string RefreshToken) : IRequest<Result<Unit, Error>>;
