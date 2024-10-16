using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.SignOut;

public record SignOutCommand(string RefreshToken) : IRequest<Result<Unit, Error>>;