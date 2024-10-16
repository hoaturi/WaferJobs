using System.Web;
using Hangfire;
using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;
using WaferJobs.Infrastructure.Services.DomainValidationService;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Business.CreateBusiness.InitiateBusinessCreation;

public class InitiateBusinessCreationCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IDomainValidationService domainValidationService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<InitiateBusinessCreationCommandHandler> logger)
    : IRequestHandler<InitiateBusinessCreationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InitiateBusinessCreationCommand command,
        CancellationToken cancellationToken)
    {
        var (userId, userEmail) = (currentUserService.GetUserId(), currentUserService.GetUserEmail());

        var userEmailDomain = userEmail.Split('@')[1];
        var businessDomain = new Uri(command.WebsiteUrl).Host.Replace("www.", "");

        if (!userEmailDomain.Equals(businessDomain)) throw new EmailDomainMismatchException(userId);
        if (await domainValidationService.IsPublicEmailDomainAsync(userEmailDomain))
            throw new PublicEmailDomainNotAllowedException(userEmailDomain, userId);

        var userHasMembership = await dbContext.BusinessMemberships.AsNoTracking()
            .AnyAsync(b => b.UserId == userId, cancellationToken);
        if (userHasMembership) throw new DuplicateMembershipException(userId);

        var token = new BusinessCreationTokenEntity
        {
            UserId = userId,
            Name = command.Name,
            WebsiteUrl = command.WebsiteUrl,
            Domain = businessDomain,
            Token = Guid.NewGuid().ToBase64String(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(TokenConstants.ExpiresIn30Minutes)
        };

        dbContext.BusinessCreationTokens.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);

        var encodedToken = HttpUtility.UrlEncode(token.Token);
        var emailDto =
            new CompleteBusinessCreationEmailDto(userId, userEmail, command.Name,
                encodedToken,
                TokenConstants.ExpiresIn30Minutes);

        backgroundJobClient.Enqueue<IEmailService>(x =>
            x.SendCompleteBusinessCreationAsync(emailDto));

        logger.LogInformation("User {UserId} initiated business creation: {BusinessName}", userId, command.Name);

        return Unit.Value;
    }
}