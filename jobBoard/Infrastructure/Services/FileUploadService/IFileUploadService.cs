namespace JobBoard.Infrastructure.Services.FileUploadService;

public interface IFileUploadService
{
    Task<string> UploadLogoAsync(string fileName, Stream fileStream, LogoTypes logoTypes);
}