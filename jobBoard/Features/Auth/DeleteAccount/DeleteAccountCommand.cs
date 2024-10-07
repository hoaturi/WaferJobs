using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.DeleteAccount;

public record DeleteAccountCommand(string Password) : IRequest<Result<Unit, Error>>;