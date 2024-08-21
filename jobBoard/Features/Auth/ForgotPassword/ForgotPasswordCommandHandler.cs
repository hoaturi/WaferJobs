using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IBackgroundJobClient backgroundJobClient
) : IRequestHandler<ForgotPasswordCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null) return Unit.Value;

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetPasswordDto = new PasswordResetEmailDto(user, resetToken);
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendPasswordResetAsync(resetPasswordDto));

        return Unit.Value;
    }
}