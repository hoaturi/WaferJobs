using System.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SignUpCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken
    )
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var isEmailInUse = await _userManager.FindByEmailAsync(request.Email);
        if (isEmailInUse is not null)
        {
            return AuthErrors.UserAlreadyExists;
        }

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };

        await _userManager.CreateAsync(user, request.Password);

        await _userManager.AddToRoleAsync(user, RoleTypes.JobSeeker.ToString());

        scope.Complete();

        return Unit.Value;
    }
}
