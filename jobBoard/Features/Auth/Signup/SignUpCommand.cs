using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.Signup;

public record SignUpCommand(string Email, string Password, string Role)
    : IRequest<Result<Unit, Error>>;