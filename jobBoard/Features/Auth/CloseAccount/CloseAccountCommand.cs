using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.CloseAccount;

public record CloseAccountCommand(string Password) : IRequest<Result<Unit, Error>>;