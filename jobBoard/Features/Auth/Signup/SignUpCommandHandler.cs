using System.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SignUpCommandHandler(
    UserManager<ApplicationUser> userManager,
    ILogger<SignUpCommandHandler> logger
) : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<SignUpCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken
    )
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var isEmailAlreadyInUse = await _userManager.FindByEmailAsync(request.Email);
        if (isEmailAlreadyInUse is not null)
        {
            _logger.LogInformation("The input email is already in use");
            return AuthErrors.UserAlreadyExists;
        }

        var newUser = new ApplicationUser { UserName = request.Email, Email = request.Email };
        await _userManager.CreateAsync(newUser, request.Password);
        await _userManager.AddToRoleAsync(newUser, RoleTypes.JobSeeker.ToString());

        _logger.LogInformation("Successfully created job seeker user with id: {}", newUser.Id);

        scope.Complete();

        return Unit.Value;
    }
}
