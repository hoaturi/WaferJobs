using MediatR;

namespace JobBoard;

public record SignUpBusinessCommand(string Email, string Password, string CompanyName)
    : IRequest<Result<Unit, Error>>;
