using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.UploadMyBusinessLogo;

public class UploadMyBusinessLogoCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService,
    IFileUploadService fileUploadService,
    ILogger<UploadMyBusinessLogoCommandHandler> logger)
    : IRequestHandler<UploadMyBusinessLogoCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        UploadMyBusinessLogoCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();

        var business = await appDbContext
            .Businesses
            .FirstOrDefaultAsync(b => b.UserId == currentUserId, cancellationToken);

        if (business is null)
        {
            logger.LogWarning("Business not found for user with id {UserId}", currentUserId);
            throw new BusinessNotFoundForUserException(currentUserId);
        }

        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{business.Id}{originalFileExtension}";

        var logoUrl = await fileUploadService.UploadFileAsync(
            fileName,
            command.File.OpenReadStream());

        business.LogoUrl = logoUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully uploaded logo for business with id {BusinessId}", business.Id);

        return Unit.Value;
    }
}