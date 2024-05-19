using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<Unit, Error>>;