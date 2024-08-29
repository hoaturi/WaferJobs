using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService)
    : IRequestHandler<ConfirmEmailChangeCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ConfirmEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var changeRequest = await dbContext.EmailChangeRequests
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Pin == command.Pin && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken) ?? throw new InvalidEmailChangePinException(userId);

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var setEmailResult = await userManager.SetEmailAsync(user, changeRequest.NewEmail);
        var setUserNameResult = await userManager.SetUserNameAsync(user, changeRequest.NewEmail);

        if (!setEmailResult.Succeeded || !setUserNameResult.Succeeded)
            throw new InvalidOperationException($"Failed to update user email with id {userId}");

        dbContext.EmailChangeRequests.Remove(changeRequest);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}