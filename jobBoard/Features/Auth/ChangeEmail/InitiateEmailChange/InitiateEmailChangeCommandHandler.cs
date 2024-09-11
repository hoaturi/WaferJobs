using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Auth.ChangeEmail.InitiateEmailChange;

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
        var (userId, userEmail) = (currentUserService.GetUserId(), currentUserService.GetUserEmail());

        if (userEmail.Equals(command.NewEmail, StringComparison.OrdinalIgnoreCase))
            return AuthErrors.EmailUnChanged;

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        if (!await userManager.CheckPasswordAsync(user, command.Password))
            return AuthErrors.InvalidCurrentPassword;

        if (await dbContext.BusinessMemberships.AsNoTracking().AnyAsync(x => x.UserId == userId, cancellationToken))
            return AuthErrors.EmailChangeNotAllowedForBusinessMembers;

        if (await dbContext.Users.AsNoTracking().AnyAsync(x => x.Email == command.NewEmail, cancellationToken))
            return AuthErrors.EmailAlreadyInUse;

        var pin = new Random().Next(PinConstants.MinValue, PinConstants.MaxValue).ToString();
        var newRequest = new EmailChangeRequestEntity
        {
            UserId = userId,
            NewEmail = command.NewEmail,
            Pin = pin,
            ExpiresAt = DateTime.UtcNow.AddMinutes(PinConstants.PinExpiryInMinutes),
            IsVerified = false
        };
        dbContext.EmailChangeRequests.Add(newRequest);

        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailChangeVerificationAsync(
            new EmailChangeVerificationEmailDto(command.NewEmail, pin)));

        return Unit.Value;
    }
}