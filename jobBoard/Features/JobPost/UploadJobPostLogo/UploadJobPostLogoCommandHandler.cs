using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Features.Business.UpdateMyBusinessLogo;
using MediatR;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

public class
    UploadJobPostLogoCommandHandler(
        IFileUploadService fileUploadService
    ) : IRequestHandler<UploadJobPostLogoCommand,
    Result<UpdateBusinessLogoResponse, Error>>
{
    public async Task<Result<UpdateBusinessLogoResponse, Error>> Handle(UploadJobPostLogoCommand command,
        CancellationToken cancellationToken)
    {
        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{Guid.NewGuid()}{originalFileExtension}";

        var uploadedLogoUrl = await fileUploadService.UploadBusinessLogoAsync(
            fileName,
            command.File.OpenReadStream());

        return new UpdateBusinessLogoResponse(uploadedLogoUrl);
    }
}