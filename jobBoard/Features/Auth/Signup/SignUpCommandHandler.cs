using System.Transactions;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobSeeker;
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
        if (existingUser is not null)
        {
            logger.LogWarning("User with email {Email} already exists", command.Email);
            return AuthErrors.UserAlreadyExists;
        }

        var newUser = new ApplicationUserEntity { UserName = command.Email, Email = command.Email };
        var createUserResult = await userManager.CreateAsync(newUser, command.Password);
        if (!createUserResult.Succeeded)
            logger.LogError("Failed to create user with email {Email}.", command.Email);

        var addRoleResult = await userManager.AddToRoleAsync(newUser, command.Role);
        if (!addRoleResult.Succeeded)
            logger.LogError("Failed to add user with email {Email} to role {Role}.", command.Email,
                command.Role);

        switch (command.Role)
        {
            case nameof(UserRoles.Business):
                await CreateBusiness(command, newUser, cancellationToken);
                break;
            case nameof(UserRoles.JobSeeker):
                await CreateJobSeeker(command, newUser, cancellationToken);
                break;
        }

        logger.LogInformation("Successfully created {UserRole} user with id {UserId}", newUser.Id, command.Role);

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

    private async Task CreateJobSeeker(SignUpCommand command,
        ApplicationUserEntity newUserEntity, CancellationToken cancellationToken)
    {
        var newJobSeeker = new JobSeekerEntity { UserId = newUserEntity.Id, Name = command.Name! };
        await appDbContext.JobSeekers.AddAsync(newJobSeeker, cancellationToken);
        var saveChangesResult = await appDbContext.SaveChangesAsync(cancellationToken);
        if (saveChangesResult == 0)
            logger.LogError("Failed to create job seeker entity for user with email {Email}", command.Email);
    }
}