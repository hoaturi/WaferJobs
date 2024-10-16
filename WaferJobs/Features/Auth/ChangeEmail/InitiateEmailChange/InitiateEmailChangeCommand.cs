using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ChangeEmail.InitiateEmailChange;

public record InitiateEmailChangeCommand(string NewEmail, string Password) : IRequest<Result<Unit, Error>>;