using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.FileUploadService;
using MediatR;

namespace JobBoard.Features.Conference.UploadConferenceLogo;

public class UploadConferenceLogoCommandHandler(IFileUploadService fileUploadService)
    : IRequestHandler<UploadConferenceLogoCommand, Result<UploadConferenceLogoResponse, Error>>
{
    public async Task<Result<UploadConferenceLogoResponse, Error>> Handle(UploadConferenceLogoCommand command,
        CancellationToken cancellationToken)
    {
        var originalFileExtension = Path.GetExtension(command.File.FileName);
        var fileName = $"{Guid.NewGuid()}{originalFileExtension}";

        var uploadedLogoUrl = await fileUploadService.UploadLogoAsync(
            fileName,
            command.File.OpenReadStream(),
            LogoTypes.Conference
        );

        return new UploadConferenceLogoResponse(uploadedLogoUrl);
    }
}