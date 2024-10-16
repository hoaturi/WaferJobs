using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Features.Business.UpdateMyBusinessLogo;
using WaferJobs.Infrastructure.Services.FileUploadService;

namespace WaferJobs.Features.JobPost.UploadJobPostLogo;

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