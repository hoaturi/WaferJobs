using System.Transactions;
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
    AppDbContext appDbContext,
    ILogger<SignUpCommandHandler> logger)
    : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand command,
        CancellationToken cancellationToken
    )
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null) return AuthErrors.UserAlreadyExists;

        var newUser = new ApplicationUserEntity { UserName = command.Email, Email = command.Email };
        await userManager.CreateAsync(newUser, command.Password);

        await userManager.AddToRoleAsync(newUser, command.Role);

        if (command.Role == nameof(UserRoles.Business))
            await CreateBusiness(command, newUser, cancellationToken);

        logger.LogInformation("Successfully created {UserRole} user with id {UserId}", command.Role, newUser.Id);

        scope.Complete();

        return Unit.Value;
    }

    private async Task CreateBusiness(SignUpCommand command,
        ApplicationUserEntity newUserEntity, CancellationToken cancellationToken)
    {
        var newBusiness = new BusinessEntity { UserId = newUserEntity.Id, Name = command.Name! };
        await appDbContext.Businesses.AddAsync(newBusiness, cancellationToken);
        var saveChangesResult = await appDbContext.SaveChangesAsync(cancellationToken);
        if (saveChangesResult == 0)
            logger.LogError("Failed to create business entity for user with email {Email}", command.Email);
    }
}