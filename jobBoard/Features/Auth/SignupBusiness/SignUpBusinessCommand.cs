using MediatR;

namespace JobBoard;

public record class SignUpBusinessCommand(string Email, string Password, string CompanyName)
    : IRequest<Result<Unit, Error>>;
