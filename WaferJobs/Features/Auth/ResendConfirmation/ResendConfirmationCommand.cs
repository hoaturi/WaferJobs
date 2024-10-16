using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ResendConfirmation;

public record ResendConfirmationCommand(string Email) : IRequest<Result<Unit, Error>>;