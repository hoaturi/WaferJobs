using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.InitiateEmailChange;

public class InitiateEmailChangeCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient
) : IRequestHandler<InitiateEmailChangeCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InitiateEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid) return AuthErrors.InvalidCurrentPassword;

        var pin = new Random().Next(100000, 999999);

        var emailChangeRequest = new EmailChangeRequestEntity
        {
            UserId = userId,
            NewEmail = command.NewEmail,
            Pin = pin,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsVerified = false
        };

        dbContext.EmailChangeRequests.Add(emailChangeRequest);

        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailChangeVerificationAsync(
            new EmailChangeVerificationDto(command.NewEmail, pin)));

        return Unit.Value;
    }
}