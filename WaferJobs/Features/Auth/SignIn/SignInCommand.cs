using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Auth.SignIn;

public record SignInCommand(string Email, string Password)
    : IRequest<Result<SignInResponse, Error>>;