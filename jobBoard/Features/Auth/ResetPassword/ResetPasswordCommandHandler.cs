using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<Unit, Error>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return AuthErrors.UserNotFound;
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            return AuthErrors.InvalidToken;
        }

        return Unit.Value;
    }
}
