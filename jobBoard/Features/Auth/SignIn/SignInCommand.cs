using MediatR;

namespace JobBoard;

public record SignInCommand(string Email, string Password)
    : IRequest<Result<SignInResponse, Error>>;
