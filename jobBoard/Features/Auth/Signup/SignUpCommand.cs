using FluentValidation;
using MediatR;

namespace JobBoard;

public record SignUpCommand(string Email, string Password, string Role)
    : IRequest<Result<Unit, Error>>;
