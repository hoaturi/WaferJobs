using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

public class
    UploadJobPostLogoCommandHandler(
        IFileUploadService fileUploadService,
        ILogger<UploadJobPostLogoCommandHandler> logger) : IRequestHandler<UploadJobPostLogoCommand,
    Result<UploadJobPostLogoResponse, Error>>
{
    public async Task<Result<UploadJobPostLogoResponse, Error>> Handle(UploadJobPostLogoCommand command,
        CancellationToken cancellationToken)
    {
        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{Guid.NewGuid()}{originalFileExtension}";

        var uploadedLogoUrl = await fileUploadService.UploadBusinessLogoAsync(
            fileName,
            command.File.OpenReadStream());

        return new UploadJobPostLogoResponse(uploadedLogoUrl);
    }
}