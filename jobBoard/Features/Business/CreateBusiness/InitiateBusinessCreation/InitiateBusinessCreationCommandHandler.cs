﻿using System.Web;
using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.DomainValidationService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.CreateBusiness.InitiateBusinessCreation;

public class InitiateBusinessCreationCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IDomainValidationService domainValidationService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<InitiateBusinessCreationCommandHandler> logger)
    : IRequestHandler<InitiateBusinessCreationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InitiateBusinessCreationCommand creationCommand,
        CancellationToken cancellationToken)
    {
        var (userId, userEmail) = (currentUserService.GetUserId(), currentUserService.GetUserEmail());

        var userEmailDomain = userEmail.Split('@')[1];
        var businessDomain = new Uri(creationCommand.WebsiteUrl).Host.Replace("www.", "");

        if (!userEmailDomain.Equals(businessDomain)) throw new EmailDomainMismatchException(userId);
        if (await domainValidationService.IsPublicEmailDomainAsync(userEmailDomain))
            throw new PublicEmailDomainNotAllowedException(userId);

        var userHasMembership = await dbContext.BusinessMembers.AsNoTracking()
            .AnyAsync(b => b.UserId == userId, cancellationToken);
        if (userHasMembership) throw new DuplicateMembershipException(userId);

        var token = new BusinessCreationTokenEntity
        {
            UserId = userId,
            Name = creationCommand.Name,
            WebsiteUrl = creationCommand.WebsiteUrl,
            Domain = businessDomain,
            Token = Guid.NewGuid().ToBase64String(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(TokenConstants.BusinessTokenExpirationInMinutes)
        };

        dbContext.BusinessCreationTokens.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);

        var encodedToken = HttpUtility.UrlEncode(token.Token);
        var emailDto =
            new BusinessCreationVerificationEmailDto(userId, userEmail, creationCommand.Name,
                encodedToken,
                TokenConstants.BusinessTokenExpirationInMinutes);

        backgroundJobClient.Enqueue<IEmailService>(x =>
            x.SendBusinessCreationVerificationAsync(emailDto));

        logger.LogInformation("User {userId} has requested to create a business {businessName}",
            userId, creationCommand.Name);

        return Unit.Value;
    }
}