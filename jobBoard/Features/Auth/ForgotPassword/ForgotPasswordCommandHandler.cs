using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger
) : IRequestHandler<ForgotPasswordCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            logger.LogWarning("Failed to find user with email: {Email}", command.Email);
            return AuthErrors.UserNotFound;
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await emailService.SendAsync(new PasswordResetEmailDto(user, resetToken));

        return Unit.Value;
    }
}