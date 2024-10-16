using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<Result<RefreshResponse, Error>>;