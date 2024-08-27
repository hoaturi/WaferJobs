using System.Security.Authentication;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.CloseAccount;

public class CloseAccountCommandCommandHandler(
    ICurrentUserService currentUserService,
    UserManager<ApplicationUserEntity> userManager,
    ILogger<CloseAccountCommandCommandHandler> logger) : IRequestHandler<CloseAccountCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CloseAccountCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordCorrect) throw new InvalidCredentialException();

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded) throw new InvalidOperationException($"Failed to delete user with id: {userId}");

        logger.LogInformation("User with id: {userId} has been deleted", userId);

        return Unit.Value;
    }
}