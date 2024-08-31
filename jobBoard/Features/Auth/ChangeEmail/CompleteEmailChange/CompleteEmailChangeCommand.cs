using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ChangeEmail.CompleteEmailChange;

public record CompleteEmailChangeCommand(int Pin) : IRequest<Result<Unit, Error>>;