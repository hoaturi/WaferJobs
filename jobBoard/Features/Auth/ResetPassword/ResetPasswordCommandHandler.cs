using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<ResetPasswordCommandHandler> logger
) : IRequestHandler<ResetPasswordCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", request.UserId);
            return AuthErrors.UserNotFound;
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid reset token for user {UserId}", request.UserId);
            return AuthErrors.InvalidToken;
        }

        _logger.LogInformation("Successfully reset password for user {UserId}", user.Id);
        return Unit.Value;
    }
}
