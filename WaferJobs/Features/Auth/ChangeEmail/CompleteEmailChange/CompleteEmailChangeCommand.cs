using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ChangeEmail.CompleteEmailChange;

public record CompleteEmailChangeCommand(string Pin) : IRequest<Result<Unit, Error>>;