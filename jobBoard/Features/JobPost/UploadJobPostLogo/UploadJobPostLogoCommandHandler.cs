using JobBoard.Common.Models;
using JobBoard.Features.Business.UpdateMyBusinessLogo;
using JobBoard.Infrastructure.Services.FileUploadService;
using MediatR;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

public class
    UploadJobPostLogoCommandHandler(
        IFileUploadService fileUploadService,
        ILogger<UploadJobPostLogoCommandHandler> logger
    ) : IRequestHandler<UploadJobPostLogoCommand,
    Result<UpdateBusinessLogoResponse, Error>>
{
    public async Task<Result<UpdateBusinessLogoResponse, Error>> Handle(UploadJobPostLogoCommand command,
        CancellationToken cancellationToken)
    {
        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{Guid.NewGuid()}{originalFileExtension}";

        var uploadedLogoUrl = await fileUploadService.UploadLogoAsync(
            fileName,
            command.File.OpenReadStream(),
            LogoTypes.Company);

        logger.LogInformation("Uploaded job post logo. Url: {UploadedLogoUrl}", uploadedLogoUrl);
        return new UpdateBusinessLogoResponse(uploadedLogoUrl);
    }
}