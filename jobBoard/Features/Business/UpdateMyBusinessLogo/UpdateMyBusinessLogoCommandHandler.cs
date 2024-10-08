using JobBoard.Common.Models;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.FileUploadService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.UpdateMyBusinessLogo;

public class
    UpdateMyBusinessLogoCommandHandler(
        AppDbContext appDbContext,
        ICurrentUserService currentUserService,
        IFileUploadService fileUploadService,
        ILogger<UpdateMyBusinessLogoCommandHandler> logger)
    : IRequestHandler<UpdateMyBusinessLogoCommand,
        Result<UpdateBusinessLogoResponse, Error>>
{
    public async Task<Result<UpdateBusinessLogoResponse, Error>> Handle(
        UpdateMyBusinessLogoCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();

        var business = await appDbContext.Businesses
            .Where(b => b.Memberships.Any(m => m.UserId == currentUserId && m.IsActive))
            .FirstOrDefaultAsync(cancellationToken) ?? throw new UserHasNoBusinessMembershipException(currentUserId);

        var membership = business.Memberships.First(m => m.UserId == currentUserId);
        if (!membership.IsAdmin) throw new BusinessMemberNotAdminException(business.Id, currentUserId);

        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{business.Id}{originalFileExtension}";

        var uploadedLogoUrl = await fileUploadService.UploadLogoAsync(
            fileName,
            command.File.OpenReadStream(),
            LogoTypes.Company);

        business.LogoUrl = uploadedLogoUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated business logo for business {BusinessId} for user {UserId}", business.Id,
            currentUserId);

        return new UpdateBusinessLogoResponse(uploadedLogoUrl);
    }
}