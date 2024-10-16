using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Auth.ForgotPassword;

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