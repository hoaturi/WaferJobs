using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.Signup;

public class SignUpCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IBackgroundJobClient backgroundJobClient,
    ILogger<SignUpCommandHandler> logger)
    : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand command,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUserEntity { UserName = command.Email, Email = command.Email };

        var createResult = await userManager.CreateAsync(user, command.Password);

        if (createResult.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
            return AuthErrors.UserAlreadyExists;

        await userManager.AddToRoleAsync(user, command.Role);

        var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmEmailDto = new ConfirmEmailDto(user, confirmToken);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailConfirmAsync(confirmEmailDto));

        logger.LogInformation("User {Email} signed up successfully", user.Email);
        return Unit.Value;
    }
}