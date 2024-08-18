using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.Signup;

public class SignUpCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    AppDbContext dbContext,
    IEmailService emailService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<SignUpCommandHandler> logger)
    : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new ApplicationUserEntity { UserName = command.Email, Email = command.Email };

            var createResult = await userManager.CreateAsync(user, command.Password);

            if (createResult.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
                return AuthErrors.UserAlreadyExists;

            await userManager.AddToRoleAsync(user, nameof(UserRoles.JobSeeker));

            var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailDto = new ConfirmEmailDto(user, confirmToken);

            backgroundJobClient.Enqueue(() => emailService.SendEmailConfirmAsync(confirmEmailDto));

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("User {Email} signed up successfully", user.Email);
            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}