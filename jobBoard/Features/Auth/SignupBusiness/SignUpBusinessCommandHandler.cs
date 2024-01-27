using System.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SignUpBusinessCommandHandler(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext,
    ILogger<SignUpBusinessCommandHandler> logger
) : IRequestHandler<SignUpBusinessCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<SignUpBusinessCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        SignUpBusinessCommand request,
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

        var newUser = new ApplicationUser { Email = request.Email, UserName = request.Email, };
        await _userManager.CreateAsync(newUser, request.Password);
        await _userManager.AddToRoleAsync(newUser, RoleTypes.Business.ToString());

        var newBusiness = new Business { UserId = newUser.Id, Name = request.CompanyName, };
        await _appDbContext.Businesses.AddAsync(newBusiness, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created business user with id: {}", newUser.Id);

        scope.Complete();

        return Unit.Value;
    }
}
