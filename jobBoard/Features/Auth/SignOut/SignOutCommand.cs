using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.SignOut;

public record SignOutCommand(string RefreshToken) : IRequest<Result<Unit, Error>>;