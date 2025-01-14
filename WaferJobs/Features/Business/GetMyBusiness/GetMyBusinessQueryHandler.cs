﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Features.Business.GetBusiness;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.Business.GetMyBusiness;

public class GetMyBusinessQueryHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMyBusinessQuery, Result<GetBusinessResponse, Error>>
{
    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetMyBusinessQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUserService.GetUserId();

        var businessResponse =
            await appDbContext
                .Businesses
                .AsNoTracking()
                .Where(b => b.Memberships.Any(m => m.UserId == currentUserId))
                .Select(b => new GetBusinessResponse(
                    b.Id,
                    b.Name,
                    b.LogoUrl,
                    b.Description,
                    b.Location,
                    b.WebsiteUrl,
                    b.TwitterUrl,
                    b.LinkedinUrl,
                    b.BusinessSize != null ? b.BusinessSize.Label : null
                ))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new UserHasNoBusinessMembershipException(currentUserId);

        return businessResponse;
    }
}