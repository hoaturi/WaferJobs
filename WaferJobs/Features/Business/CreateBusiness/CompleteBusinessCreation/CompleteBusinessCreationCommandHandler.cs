﻿using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Slugify;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Business.CreateBusiness.CompleteBusinessCreation;

public class CompleteBusinessCreationCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<CompleteBusinessCreationCommandHandler> logger)
    : IRequestHandler<CompleteBusinessCreationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CompleteBusinessCreationCommand command,
        CancellationToken cancellationToken)
    {
        var (userId, userEmail) = (currentUserService.GetUserId(), currentUserService.GetUserEmail());

        var token = await dbContext.BusinessCreationTokens
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Token == command.Token && x.ExpiresAt >= DateTime.UtcNow,
                cancellationToken);

        if (token is null || token.IsUsed)
        {
            logger.LogWarning("User {UserId} attempted to create business with invalid token: {token}", userId, token);
            return BusinessErrors.InvalidCreationToken;
        }

        var userEmailDomain = userEmail.Split('@')[1];
        if (token.Domain != userEmailDomain) throw new EmailDomainMismatchException(userId);

        token.IsUsed = true;
        token.UsedAt = DateTime.UtcNow;

        var slugHelper = new SlugHelper();

        var business = new BusinessEntity
        {
            Name = token.Name,
            WebsiteUrl = token.WebsiteUrl,
            Domain = token.Domain,
            IsActive = false,
            Slug = slugHelper.GenerateSlug(token.Name)
        };

        var member = new BusinessMembershipEntity
        {
            UserId = userId,
            BusinessId = business.Id,
            FirstName = command.Dto.FirstName,
            LastName = command.Dto.LastName,
            Title = command.Dto.Title,
            IsAdmin = true,
            JoinedAt = DateTime.UtcNow,
            IsActive = false
        };

        dbContext.Businesses.Add(business);
        business.Memberships.Add(member);

        await dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = new BusinessCreationReviewEmailDto(userEmail, business.Id);
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessCreationReviewAsync(emailDto));

        logger.LogInformation("Business {BusinessId} created by user: {UserId}", business.Id, userId);
        return Unit.Value;
    }
}