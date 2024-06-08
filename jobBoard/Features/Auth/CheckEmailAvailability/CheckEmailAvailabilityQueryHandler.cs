using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.CheckEmailAvailability;

public class CheckEmailAvailabilityQueryHandler(
    UserManager<ApplicationUserEntity> userManager
) : IRequestHandler<CheckEmailAvailabilityQuery, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CheckEmailAvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(query.Email);

        if (existingUser is not null) return AuthErrors.UserAlreadyExists;

        return Unit.Value;
    }
}