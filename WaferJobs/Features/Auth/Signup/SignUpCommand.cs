using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.Signup;

public record SignUpCommand(string Email, string Password, string Role)
    : IRequest<Result<Unit, Error>>;