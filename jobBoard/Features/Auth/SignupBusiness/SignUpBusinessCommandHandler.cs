using System.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SignUpBusinessCommandHandler(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext
) : IRequestHandler<SignUpBusinessCommand, Result<Unit, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<Unit, Error>> Handle(
        SignUpBusinessCommand request,
        CancellationToken cancellationToken
    )
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var isEmailInUse = await _userManager.FindByEmailAsync(request.Email);

        if (isEmailInUse is not null)
        {
            return AuthErrors.UserAlreadyExists;
        }

        var user = new ApplicationUser { Email = request.Email, UserName = request.Email, };
        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, RoleTypes.Business.ToString());

        var business = new Business { UserId = user.Id, Name = request.CompanyName, };

        await _appDbContext.Businesses.AddAsync(business, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return Unit.Value;
    }
}
