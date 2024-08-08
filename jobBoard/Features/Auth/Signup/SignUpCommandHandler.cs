using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.Signup;

public class SignUpCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    AppDbContext dbContext,
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

            await userManager.AddToRoleAsync(user, command.Role);

            if (command.Role == nameof(UserRoles.Business)) await CreateBusiness(command, user, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Successfully created {UserRole} user with id {UserId}", command.Role, user.Id);
            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task CreateBusiness(SignUpCommand command,
        ApplicationUserEntity newUserEntity, CancellationToken cancellationToken)
    {
        var newBusiness = new BusinessEntity { UserId = newUserEntity.Id, Name = command.Name! };
        await dbContext.Businesses.AddAsync(newBusiness, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}