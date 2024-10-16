using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<Result<Unit, Error>>;