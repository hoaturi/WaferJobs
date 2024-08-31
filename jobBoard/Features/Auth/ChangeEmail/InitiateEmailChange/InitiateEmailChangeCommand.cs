using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ChangeEmail.InitiateEmailChange;

public record InitiateEmailChangeCommand(string NewEmail, string Password) : IRequest<Result<Unit, Error>>;