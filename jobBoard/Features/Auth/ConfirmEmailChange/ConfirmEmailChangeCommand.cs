using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

public record ConfirmEmailChangeCommand(int Pin) : IRequest<Result<Unit, Error>>;