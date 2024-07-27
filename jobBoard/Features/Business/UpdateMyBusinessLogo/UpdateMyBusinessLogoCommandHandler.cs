using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.FileUploadService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.UpdateMyBusinessLogo;

public class UpdateMyBusinessLogoCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService,
    IFileUploadService fileUploadService,
    ILogger<UpdateMyBusinessLogoCommandHandler> logger)
    : IRequestHandler<UpdateMyBusinessLogoCommand, Result<UpdateBusinessLogoResponse, Error>>
{
    public async Task<Result<UpdateBusinessLogoResponse, Error>> Handle(
        UpdateMyBusinessLogoCommand command,
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

        var uploadedLogoUrl = await fileUploadService.UploadBusinessLogoAsync(
            fileName,
            command.File.OpenReadStream());

        business.LogoUrl = uploadedLogoUrl;

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new UpdateBusinessLogoResponse(uploadedLogoUrl);
    }
}