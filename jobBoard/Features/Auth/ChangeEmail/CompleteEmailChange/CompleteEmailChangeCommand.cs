using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ChangeEmail.CompleteEmailChange;

public record CompleteEmailChangeCommand(string Pin) : IRequest<Result<Unit, Error>>;