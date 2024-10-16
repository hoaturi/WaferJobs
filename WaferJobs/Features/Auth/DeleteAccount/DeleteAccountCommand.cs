using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.DeleteAccount;

public record DeleteAccountCommand(string Password) : IRequest<Result<Unit, Error>>;