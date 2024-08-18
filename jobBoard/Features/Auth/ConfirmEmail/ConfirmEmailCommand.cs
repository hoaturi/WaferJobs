using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<Result<Unit, Error>>;