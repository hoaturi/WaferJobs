using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender
) : IRequestHandler<ForgotPasswordCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<Result<Unit, Error>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return AuthErrors.UserNotFound;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _emailSender.SendPasswordResetEmailAsync(new EmailDto(user, token));

        return Unit.Value;
    }
}
