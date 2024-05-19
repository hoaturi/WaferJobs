using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.SignIn;

public record SignInCommand(string Email, string Password)
    : IRequest<Result<SignInResponse, Error>>;