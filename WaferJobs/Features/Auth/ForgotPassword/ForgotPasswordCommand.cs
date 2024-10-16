using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<Unit, Error>>;