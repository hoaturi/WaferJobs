using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger
) : IRequestHandler<ForgotPasswordCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logger.LogInformation("User with the input email not found");
            return AuthErrors.UserNotFound;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _emailService.SendPasswordResetEmailAsync(new EmailDto(user, token));

        return Unit.Value;
    }
}
