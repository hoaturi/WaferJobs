using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JobBoard.Features.Auth.Signup;

public class SignUpCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    AppDbContext dbContext,
    ILogger<SignUpCommandHandler> logger)
    : IRequestHandler<SignUpCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignUpCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new ApplicationUserEntity { UserName = command.Email, Email = command.Email };

            await userManager.CreateAsync(user, command.Password);
            await userManager.AddToRoleAsync(user, command.Role);

            if (command.Role == nameof(UserRoles.Business))
                await CreateBusiness(command, user, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Successfully created {UserRole} user with id {UserId}", command.Role, user.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (ex is DbUpdateException
                {
                    InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
                })
            {
                logger.LogError("Failed to create {UserRole} user with email {Email}. Email already exists",
                    command.Role, command.Email);
                return AuthErrors.UserAlreadyExists;
            }
        }

        return Unit.Value;
    }

    private async Task CreateBusiness(SignUpCommand command,
        ApplicationUserEntity newUserEntity, CancellationToken cancellationToken)
    {
        var newBusiness = new BusinessEntity { UserId = newUserEntity.Id, Name = command.Name! };
        await dbContext.Businesses.AddAsync(newBusiness, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}